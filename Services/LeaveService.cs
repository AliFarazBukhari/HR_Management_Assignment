using Entities;
using Repository;
using System.Data.Common;

namespace Services
{
    public class LeaveService
    {

        private readonly HRRepository _repository;


        public LeaveService(
            HRRepository repository)
        {
            _repository = repository;
        }



        public async Task<Response<bool>> SubmitLeaveAsync(LeaveRequest request)
        {
            Response<bool> response = new() { HttpStatusCode = System.Net.HttpStatusCode.OK, Data = true };

            try
            {
                // 1. FETCH OR AUTO-REGISTER EMPLOYEE
                var employeeExists = await _repository.GetEmployeeByNumberAsync($"EMP{request.EmployeeId}");

                if (employeeExists == null)
                {
                    employeeExists = new Employee
                    {
                        FullName = $"User {request.EmployeeId}",
                        Email = $"user{request.EmployeeId}@company.com",
                        EmployeeNumber = $"EMP{request.EmployeeId}"
                    };
                    await _repository.AddEmployeeAsync(employeeExists);

                    // After adding the employee, initialize their balances

                    var allLeaveTypes = await _repository.GetLeaveTypesAsync();

                    allLeaveTypes.ForEach(type =>
                    {
                        var newBalance = new LeaveBalance
                        {
                            Employee = employeeExists,
                            EmployeeId = employeeExists.Id,
                            LeaveTypeId = type.Id,
                            Balance = 22 // Set the initial 22 days
                        };

                        _repository.AddBalanceAsync(newBalance).GetAwaiter().GetResult();

                    });
                }

                // 2. AUTO-REGISTER LEAVE TYPE
                var leaveTypeExists = await _repository.LeaveTypeExistsAsync(request.LeaveTypeId);

                if (!leaveTypeExists)
                {
                    await _repository.AddLeaveTypeAsync(new LeaveType
                    {
                        Id = request.LeaveTypeId,
                        Name = $"Leave Type {request.LeaveTypeId}",
                        MonthlyAccrualRate = 1.00m
                    });
                }

                // 3. BALANCE CHECK
                int daysRequested = (request.EndDate - request.StartDate).Days + 1;

                var balance = await _repository.GetBalanceAsync(employeeExists.Id, request.LeaveTypeId);

                if (balance == null || balance.Balance < daysRequested)
                {
                    response.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.ResponseMessage = "Insufficient leave balance.";
                    response.Data = false;
                    response.ResponseCode = ResponseCode.LeaveExhausted;

                    return response;
                }

                // 4. CONFLICT DETECTION
                if (await _repository.HasConflictAsync(employeeExists.Id, request.StartDate, request.EndDate))
                {
                    response.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.ResponseMessage = "Leave overlaps with approved leave.";
                    response.ResponseCode = ResponseCode.OverlappingLeave;

                    response.Data = false;
                    return response;
                }



                // 5. SAVE REQUEST (Crucial: Clean the request object to prevent tracking conflicts)
                _repository.SetEmployeeUnchanged(employeeExists);
                request.EmployeeId = employeeExists.Id;
                request.Employee = employeeExists; // Clear navigation property so EF doesn't try to re-insert Employee
                request.LeaveType = null; // Clear navigation property
                request.Status = LeaveRequestStatus.Pending;
                request.CreatedOn = DateTime.UtcNow;
                request.DaysRequested = daysRequested;

                await _repository.AddLeaveRequestAsync(request);
            }
            catch (Exception ex)
            {
                response.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.ResponseMessage = ex.Message;
                response.Data = false;


            }

            return response;
        }

        public async Task<Response<bool>> ApproveLeaveAsync(int leaveRequestId)
        {
            Response<bool> response = new()
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                Data = true
            };

            try
            {
                var request = await _repository.GetLeaveRequestAsync(leaveRequestId);

                if (request == null)
                {
                    response.HttpStatusCode = System.Net.HttpStatusCode.NotFound;
                    response.ResponseMessage = "Leave request not found";
                    response.Data = false;

                    return response;
                }

                var balance =
                    await _repository.GetBalanceAsync(
                        request.EmployeeId,
                        request.LeaveTypeId);

                if (balance == null)
                {
                    response.HttpStatusCode = System.Net.HttpStatusCode.NotFound;
                    response.ResponseMessage = "Leave balance not found";
                    response.Data = false;

                    return response;
                }

                if (balance.Balance < request.DaysRequested)
                {
                    response.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;

                    response.ResponseMessage =
                        "Insufficient leave balance";

                    response.Data = false;

                    return response;
                }

                request.Status =
                    LeaveRequestStatus.Approved;

                balance.Balance -=
                    request.DaysRequested;

                await _repository
                    .UpdateBalanceAsync(balance);

                await _repository
                    .UpdateLeaveRequestAsync(request);
            }
            catch (Exception ex)
            {
                response.HttpStatusCode =
                    System.Net.HttpStatusCode.InternalServerError;

                response.ResponseMessage =
                    ex.Message;

                response.Data = false;
            }

            return response;
        }


        public async Task RejectLeaveAsync(
            int id,
            string comment)
        {


            var request =
                await _repository
                .GetLeaveRequestAsync(id);



            if (request == null)
                throw new Exception(
                "Leave request not found");




            request.Status =
                LeaveRequestStatus.Rejected;



            request.RejectionComment =
                comment;



            await _repository
                .UpdateLeaveRequestAsync(request);

        }

        public async Task CancelLeaveAsync(
            int id)
        {

            var request =
                await _repository
                .GetLeaveRequestAsync(id);



            if (request == null)
                throw new Exception(
                "Leave request not found");




            if (request.Status ==
                LeaveRequestStatus.Approved)
            {


                var balance =
                await _repository.GetBalanceAsync(
                    request.EmployeeId,
                    request.LeaveTypeId);



                if (balance != null)
                {
                    balance.Balance +=
                        request.DaysRequested;


                    await _repository
                    .UpdateBalanceAsync(balance);
                }

            }



            request.Status =
                LeaveRequestStatus.Cancelled;



            await _repository
                .UpdateLeaveRequestAsync(request);

        }

        public async Task BulkApproveAsync(
            List<int> ids)
        {

            foreach (var id in ids)
            {
                await ApproveLeaveAsync(id);
            }

        }

        public async Task<Response<List<LeaveBalance>>> GetDashboardBalanceAsync(int employeeId)
        {
            Response<List<LeaveBalance>> response = new()
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK
            };

            try
            {
                response.Data = await _repository.GetEmployeeBalancesAsync(-1);
            }
            catch (Exception ex)
            {
                response.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }

        public async Task<Response<List<LeaveRequest>>> GetDashboardRequestsAsync(
        int employeeId,
        LeaveRequestStatus? status,
        int? leaveTypeId,
        DateTime? from,
        DateTime? to)
        {

            Response<List<LeaveRequest>> toReturn = new() { HttpStatusCode = System.Net.HttpStatusCode.OK };

            try
            {
                toReturn.Data = await _repository.GetEmployeeLeaveRequestsAsync(employeeId, status, leaveTypeId, from, to);
            }
            catch (Exception ex)
            {
                toReturn.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
                toReturn.ResponseMessage = ex.Message;
            }


            return toReturn;
        }

    }
}
