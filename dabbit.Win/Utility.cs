using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows.Controls;
using System.Threading;
namespace dabbit.Base
{

    public static class StringUtility
    {
        public static string ToMd5(this string me)
        {
            byte[] bytePassword = Encoding.UTF8.GetBytes(me);

            using (MD5 md5 = MD5.Create())
            {
                byte[] byteHashedPassword = md5.ComputeHash(bytePassword);
                return BitConverter.ToString(byteHashedPassword).Replace("-", "");
            }
        }

        public static void InvokeIfRequired(this System.Windows.UIElement control, Action action)
        {
            if (!control.Dispatcher.CheckAccess())
            {
                try
                {
                    control.Dispatcher.Invoke(action);
                }
                catch (TaskCanceledException)
                { } // TODO: Handle this better!
            }
            else
            {
                action();
            }
        }
    }
}
