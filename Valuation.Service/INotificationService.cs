using LanguageExt.ClassInstances;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public interface INotificationService
    {
        Task Send(string subject, string message);
    }
}
