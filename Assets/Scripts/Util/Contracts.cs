using System;
using System.Diagnostics;

namespace Util {
    public static class Contracts {
        private const bool EnableContracts = true;

        private static void ThrowOn<E> (bool condition, string message) where E : ContractsException, new () {
            if (!condition)
                throw new E {
                    Error = message
                };
        }

        [Conditional("DEBUG")]
        public static void Requires<E> (bool condition, string message = "Precondition Failed")
            where E : ContractsException, new () {
            if (EnableContracts) {
                ThrowOn<E> (condition, message);
            }
        }

        [Conditional("DEBUG")]
        public static void Requires (bool condition, string message = "Precondition Failed") {
            if (EnableContracts) {
                ThrowOn<ContractsException> (condition, message);
            }
        }

        [Conditional("DEBUG")]
        public static void Ensures<E> (bool condition, string message = "Postcondition Failed")
            where E : ContractsException, new () {
            if (EnableContracts) {
                ThrowOn<E> (condition, message);
            }
        }

        [Conditional("DEBUG")]
        public static void Ensures (bool condition, string message = "Postcondition Failed") {
            if (EnableContracts) {
                ThrowOn<ContractsException> (condition, message);
            }
        }

        [Conditional("DEBUG")]
        public static void Asserts<E> (bool condition, string message = "Assertion Failed")
            where E : ContractsException, new () {
            if (EnableContracts) {
                ThrowOn<E> (condition, message);
            }
        }

        [Conditional("DEBUG")]
        public static void Asserts (bool condition, string message = "Assertion Failed") {
            if (EnableContracts) {
                ThrowOn<ContractsException> (condition, message);
            }
        }

        public class ContractsException : Exception {
            public string Error { get; set; } = "";
            public override string Message => Error;
        }
    }
}