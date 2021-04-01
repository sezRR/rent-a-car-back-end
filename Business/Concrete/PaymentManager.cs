using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    public class PaymentManager : IPaymentService
    {
        public IResult Pay()
        {
            // In our database we not keep informations about credit cards so i added algorithmic operation

            var test = new Random().Next(2);
            if (test != 1)
            {
                return new ErrorResult(Messages.PaymentFailed);
            } else
            {
                return new SuccessResult(Messages.PaymentSuccessful);
            }
        }
    }
}
