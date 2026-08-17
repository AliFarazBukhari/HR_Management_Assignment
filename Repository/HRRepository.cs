using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class HRRepository
    {
        private readonly HrDbContext _context;

        public HRRepository(HrDbContext context)
        {
            _context = context;
        }


        // Employees
        public async Task<Employee?> GetEmployeeAsync(int id)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // Leave Types
        public async Task<List<LeaveType>> GetLeaveTypesAsync()
        {
            return await _context.LeaveTypes
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<LeaveType?> GetLeaveTypeAsync(int id)
        {
            return await _context.LeaveTypes
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateLeaveTypeAsync(
            LeaveType leaveType)
        {
            _context.LeaveTypes.Update(leaveType);

            await _context.SaveChangesAsync();
        }


        // Leave Requests Dashboard
        public async Task<List<LeaveRequest>>
            GetEmployeeLeaveRequestsAsync(
            int employeeId,
            LeaveRequestStatus? status,
            int? leaveTypeId,
            DateTime? from,
            DateTime? to)
        {


            var query = _context.LeaveRequests
                .Include(x => x.LeaveType)
                .Where(a=> 1==1);
               


            if(employeeId != -1)
            {
                query = query.Where(x =>x.EmployeeId == employeeId);
            }


            if (status.HasValue)
            {
                query = query.Where(x =>
                    x.Status == status.Value);
            }



            if (leaveTypeId.HasValue)
            {
                query = query.Where(x =>
                    x.LeaveTypeId == leaveTypeId);
            }



            if (from.HasValue)
            {
                query = query.Where(x =>
                    x.StartDate >= from);
            }



            if (to.HasValue)
            {
                query = query.Where(x =>
                    x.EndDate <= to);
            }



            return await query
                .OrderByDescending(x => x.CreatedOn)
                .ToListAsync();

        }

        public async Task<LeaveRequest?>  GetLeaveRequestAsync(int id)
        {

            return await _context.LeaveRequests
                .Include(x => x.Employee)
                .Include(x => x.LeaveType)
                .FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task AddLeaveRequestAsync(
            LeaveRequest request)
        {

            await _context.LeaveRequests.AddAsync(request);
            await _context.SaveChangesAsync();

        }


        public async Task UpdateLeaveRequestAsync(
            LeaveRequest request)
        {

            _context.LeaveRequests.Update(request);

            await _context.SaveChangesAsync();

        }


        // Approval
        public async Task<List<LeaveRequest>>
            GetPendingRequestsAsync()
        {

            return await _context.LeaveRequests
                .Include(x => x.Employee)
                .Include(x => x.LeaveType)
                .Where(x =>
                    x.Status == LeaveRequestStatus.Pending)
                .ToListAsync();

        }




        // Leave Balance
        public async Task<List<LeaveBalance>>
            GetEmployeeBalancesAsync(
            int employeeId)
        {

            return await _context.LeaveBalances
                .Include(x => x.LeaveType)
                //.Where(x => x.EmployeeId == employeeId)
                .ToListAsync();

        }

        public async Task<LeaveBalance?>
            GetBalanceAsync(
            int employeeId,
            int leaveTypeId)
        {

            return await _context.LeaveBalances
                .FirstOrDefaultAsync(x =>
                    x.EmployeeId == employeeId &&
                    x.LeaveTypeId == leaveTypeId);

        }


        public async Task UpdateBalanceAsync(
            LeaveBalance balance)
        {

            _context.LeaveBalances.Update(balance);

            await _context.SaveChangesAsync();

        }


        // Conflict Detection
        public async Task<bool>
            HasConflictAsync(
            int employeeId,
            DateTime start,
            DateTime end)
        {


            return await _context.LeaveRequests.AnyAsync(x =>

                x.EmployeeId == employeeId &&

                x.Status == LeaveRequestStatus.Approved &&

                x.StartDate <= end &&

                x.EndDate >= start

            );

        }


        // CSV Export
        public async Task<List<LeaveRequest>>
            GetLeaveHistoryAsync(
            int employeeId)
        {

            return await _context.LeaveRequests
                .Include(x => x.LeaveType)
                .Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.CreatedOn)
                .ToListAsync();

        }


        public async Task<Employee?> GetEmployeeByNumberAsync(string employeeNumber)
        {
            return await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber);
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> LeaveTypeExistsAsync(int id)
        {
            return await _context.LeaveTypes.AnyAsync(t => t.Id == id);
        }

        public async Task AddLeaveTypeAsync(LeaveType type)
        {
            await _context.LeaveTypes.AddAsync(type);
            await _context.SaveChangesAsync();
        }

        public async Task AddBalanceAsync(LeaveBalance balance)
        {
            await _context.LeaveBalances.AddAsync(balance);
            await _context.SaveChangesAsync();
        }

        public void SetEmployeeUnchanged(Employee employee)
        {
            // This tells EF: "This record exists, do not try to INSERT or UPDATE it"
            _context.Entry(employee).State = EntityState.Unchanged;
        }
    }
}
