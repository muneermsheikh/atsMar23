using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class UserToReturnDto
    {
        public UserDto UserDto {get; set;}
        public ICollection<TaskDashboardDto> dashboardTasks {get; set;}
    }
}