using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RapidWeb.Models;

namespace RapidWeb.ViewModel
{
    public class DailyExpenseViewModel
    {
        public IEnumerable<Employee> Employees { get; set; }

        public DailyExpense DailyExpense { get; set; }
    }
}