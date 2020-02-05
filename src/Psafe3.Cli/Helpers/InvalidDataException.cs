/*
 * ---------------------------------------------------------
 * InvalidDataException.cs  wrapper
 *
 * by Alphons van der Heijden
 * ---------------------------------------------------------
 * Comments:
 *  It works, thats it
 * ---------------------------------------------------------
 *
 */

using System;

namespace Psafe3.Cli.Helpers
{
    public class InvalidDataException : Exception
    {
        public InvalidDataException(
            string Message)
            : base(Message)
        {
        }
    }
}