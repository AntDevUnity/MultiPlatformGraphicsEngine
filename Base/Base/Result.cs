using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpGe.Base
{
    /// <summary>
    /// Result contains the result and/or exception caused.
    /// </summary>
    public class Result
    {

        /// <summary>
        /// True if succesfull, false if-not.
        /// </summary>
        public bool Success
        {
            get;
            set;
        }

        /// <summary>
        /// The exception caused, if any.
        /// </summary>
        public Exception ActualException
        {
            get;
            set;
        }

        /// <summary>
        /// constructs the result.
        /// </summary>
        /// <param name="success">true if succesfull, false if not.</param>
        /// <param name="exp">exception caused if any.</param>
        public Result(bool success,Exception exp = null)
        {
            Success = success;
            ActualException = exp;
        }

    }
}
