using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace HR_Management_Assignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HRLeaveController : ControllerBase
    {

        private readonly LeaveService _leaveService;


        public HRLeaveController(
            LeaveService leaveService)
        {
            _leaveService = leaveService;
        }


        // ===============================
        // Employee Dashboard
        // ===============================
        [HttpGet("dashboard/{employeeId}")]
        public async Task<IActionResult> Dashboard(
            int employeeId,
            LeaveRequestStatus? status,
            int? leaveTypeId,
            DateTime? from,
            DateTime? to)
        {


            var leaveRequests = await _leaveService
                .GetDashboardRequestsAsync(
                    -1,
                    status,
                    leaveTypeId,
                    from,
                    to);



            var balance = await _leaveService
                .GetDashboardBalanceAsync(
                    employeeId);
            var toReturn = new
            {

                Requests = leaveRequests,

                Balance = balance,
                //ResponseMessage = leaveRequests.ResponseMessage + balance.ResponseMessage

            };

            return Ok(toReturn);


        }

        // ===============================
        // Apply Leave
        // ===============================
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyLeave([FromBody] LeaveRequest request)
        {


           var resp = await _leaveService.SubmitLeaveAsync(request);

            return Ok(new
            {
                message = "Leave request submitted successfully",
                responseCode = resp.ResponseCode
            });


        }

        // ===============================
        // Approval Page
        // ===============================
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {

            await _leaveService.ApproveLeaveAsync(id);
            
            return Ok(new
            {

                message =
                "Leave approved"

            });

        }

        [HttpPost("reject/{id}")]
        public async Task<IActionResult> Reject(
            int id,
            [FromBody] string comment)
        {


            await _leaveService
                .RejectLeaveAsync(
                    id,
                    comment);



            return Ok(new
            {

                message =
                "Leave rejected"

            });

        }


        // ===============================
        // Bulk Approval
        // ===============================
        [HttpPost("bulk-approve")]
        public async Task<IActionResult> BulkApprove(
            [FromBody] List<int> ids)
        {


            await _leaveService
                .BulkApproveAsync(ids);



            return Ok(new
            {

                message =
                "Leaves approved"

            });


        }


        // ===============================
        // Cancel Leave
        // ===============================
        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> Cancel(
            int id)
        {


            await _leaveService
                .CancelLeaveAsync(id);



            return Ok(new
            {

                message =
                "Leave cancelled"

            });


        }

        // ===============================
        // Leave Balance Widget
        // ===============================
        [HttpGet("balance/{employeeId}")]
        public async Task<IActionResult> Balance(
            int employeeId)
        {


            var result =
                await _leaveService
                .GetDashboardBalanceAsync(employeeId);



            return Ok(result);


        }

    }
}
