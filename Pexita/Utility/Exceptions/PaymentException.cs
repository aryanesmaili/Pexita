namespace Pexita.Utility.Exceptions
{
    public class PaymentException : Exception
    {

        public class UnexpectedErrorException : Exception
        {
            public UnexpectedErrorException() : base("Unexpected Error!")
            {
            }
        }

        public class UserBlockedException : Exception
        {
            public UserBlockedException() : base("User Banned")
            {
            }
        }

        public class ApiKeyNotFoundException : Exception
        {
            public ApiKeyNotFoundException() : base("API Key Not Found")
            {
            }
        }

        public class IpMismatchException : Exception
        {
            public IpMismatchException(string ipAddress) : base($"Your request was sent from {ipAddress} which is not listed in the webservice!")
            {
            }
        }

        public class WebServiceNotApprovedException : Exception
        {
            public WebServiceNotApprovedException() : base("WebService is not yet approved!")
            {
            }
        }

        public class ServiceNotAvailableException : Exception
        {
            public ServiceNotAvailableException() : base("Service is Not AvailAble")
            {
            }
        }

        public class BankAccountNotApprovedException : Exception
        {
            public BankAccountNotApprovedException() : base("BankAccount Connected to the WebService is not yet Approved.")
            {
            }
        }

        public class WebServiceNotFoundException : Exception
        {
            public WebServiceNotFoundException() : base("WebService not found!")
            {
            }
        }

        public class WebServiceAuthenticationFailedException : Exception
        {
            public WebServiceAuthenticationFailedException() : base("WebService Authentication Failed.")
            {
            }
        }

        public class BankAccountInactiveException : Exception
        {
            public BankAccountInactiveException() : base("Bank Account Related to this WebService is Inactive/Disabled.")
            {
            }
        }

        public class TransactionIdNotEmptyException : Exception
        {
            public TransactionIdNotEmptyException() : base("TransactionID Field Must Not Be Empty!")
            {
            }
        }

        public class OrderIdNotEmptyException : Exception
        {
            public OrderIdNotEmptyException() : base("Order Field Must Not Be Empty")
            {
            }
        }

        public class AmountNotEmptyException : Exception
        {
            public AmountNotEmptyException() : base("Amount field Must Not Be Empty")
            {
            }
        }

        public class AmountLessThanMinimumException : Exception
        {
            public AmountLessThanMinimumException(int minAmount) : base($"Your Transaction Amount Cannot Be Less Than {minAmount}")
            {

            }
        }

        public class AmountExceedsMaximumException : Exception
        {
            public AmountExceedsMaximumException(int maxAmount) : base($" Your Transaction Amount Cannot Be More Than {maxAmount}")
            {
            }
        }

        public class AmountExceedsLimitException : Exception
        {
            public AmountExceedsLimitException() : base("Amount Is Above The Limit")
            {
            }
        }

        public class CallbackAddressNotEmptyException : Exception
        {
            public CallbackAddressNotEmptyException() : base("CallBack Address Cannot Be Empty")
            {
            }
        }

        public class CallbackDomainMismatchException : Exception
        {
            public CallbackDomainMismatchException() : base($"Your Request Is Sent From a domain which is not Listed in the Service")
            {
            }
        }

        public class InvalidCallbackAddressException : Exception
        {
            public InvalidCallbackAddressException() : base("CallBack Address is not Valid")
            {
            }
        }
        // TODO: Translate more errors
        public class InvalidTransactionStatusFilterException : Exception
        {
            public InvalidTransactionStatusFilterException() : base("فیلتر وضعیت تراکنش ها می بایست آرایه ای (لیستی) از وضعیت های مجاز در مستندات باشد.")
            {
            }
        }

        public class InvalidPaymentDateFilterException : Exception
        {
            public InvalidPaymentDateFilterException() : base("فیلتر تاریخ پرداخت می بایست آرایه ای شامل المنت های min و max از نوع timestamp باشد.")
            {
            }
        }

        public class InvalidSettlementDateFilterException : Exception
        {
            public InvalidSettlementDateFilterException() : base("فیلتر تاریخ تسویه می بایست آرایه ای شامل المنت های min و max از نوع timestamp باشد.")
            {
            }
        }

        public class InvalidTransactionFilterException : Exception
        {
            public InvalidTransactionFilterException() : base("فیلتر تراکنش صحیح نمی باشد.")
            {
            }
        }

        public class TransactionNotCreatedException : Exception
        {
            public TransactionNotCreatedException() : base("تراکنش ایجاد نشد.")
            {
            }
        }

        public class EmptyResultException : Exception
        {
            public EmptyResultException() : base("استعلام نتیجه ای نداشت.")
            {
            }
        }

        public class PaymentConfirmationNotPossibleException : Exception
        {
            public PaymentConfirmationNotPossibleException() : base("تایید پرداخت امکان پذیر نیست.")
            {
            }
        }

        public class PaymentConfirmationTimeoutException : Exception
        {
            public PaymentConfirmationTimeoutException() : base("مدت زمان تایید پرداخت سپری شده است.")
            {
            }
        }

    }

}
