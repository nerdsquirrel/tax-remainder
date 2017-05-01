using PayPal;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tax_Reminder.App_Start;
using Tax_Reminder.Models;

namespace Tax_Reminder.Infrastructure.Helper
{
    public static class PaypalPayment
    {
        public static bool PayWithCreditCard(TaxInformationModel taxInformationModel)
        {
           
            ItemList itemList = new ItemList
            {
                items = new List<Item>
                {
                    new Item
                    {
                        name = "Tax",
                        price = "1.99",
                        currency = "USD",
                        quantity = "1"
                    }
                }
            };

            //Address for the payment
            Address billingAddress = new Address();
            billingAddress.city = "NewYork";
            billingAddress.country_code = "US";
            billingAddress.line1 = "23rd street kew gardens";
            billingAddress.postal_code = "43210";
            billingAddress.state = "NY";



            //Now Create an object of credit card and add above details to it
            CreditCard creditCard = new CreditCard();
            creditCard.billing_address = billingAddress;
            //creditCard.cvv2 = "874";
            creditCard.expire_month = taxInformationModel.Expiry.Month;
            creditCard.expire_year = taxInformationModel.Expiry.Year;
            //creditCard.first_name = "Aman";
            //creditCard.last_name = "Thakur";
            //creditCard.number = "6011000990139424";
            creditCard.number = taxInformationModel.CardNumber;
            creditCard.type = "discover";

            // Specify details of your payment amount.
            Details details = new Details();
            //details.shipping = "1";
            details.subtotal = "1.99";
            //details.tax = "1";

            // Specify your total payment amount and assign the details object
            Amount amount = new Amount();
            amount.currency = "USD";
            // Total = shipping tax + subtotal.
            amount.total = "1.99";
            amount.details = details;

            // Now make a trasaction object and assign the Amount object
            Transaction transaction = new Transaction();
            transaction.amount = amount;
            transaction.description = "Description about the payment amount.";
            transaction.item_list = itemList;
            transaction.invoice_number = "your invoice number which you are generating";

            // Now, we have to make a list of trasaction and add the trasactions object
            // to this list. You can create one or more object as per your requirements

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(transaction);

            // Now we need to specify the FundingInstrument of the Payer
            // for credit card payments, set the CreditCard which we made above

            FundingInstrument fundInstrument = new FundingInstrument();
            fundInstrument.credit_card = creditCard;

            // The Payment creation API requires a list of FundingIntrument

            List<FundingInstrument> fundingInstrumentList = new List<FundingInstrument>();
            fundingInstrumentList.Add(fundInstrument);

            // Now create Payer object and assign the fundinginstrument list to the object
            Payer payer = new Payer();
            payer.funding_instruments = fundingInstrumentList;
            payer.payment_method = "credit_card";

            // finally create the payment object and assign the payer object & transaction list to it
            Payment payment = new Payment();
            payment.intent = "sale";
            payment.payer = payer;
            payment.transactions = transactions;


            //getting context from the paypal
            //basically we are sending the clientID and clientSecret key in this function
            //to the get the context from the paypal API to make the payment
            //for which we have created the object above.

            //Basically, apiContext object has a accesstoken which is sent by the paypal
            //to authenticate the payment to facilitator account.
            //An access token could be an alphanumeric string

            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            //Create is a Payment class function which actually sends the payment details
            //to the paypal API for the payment. The function is passed with the ApiContext
            //which we received above.

            //Payment createdPayment = null;
            //try
            //{
            //    createdPayment = payment.Create(apiContext);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
             Payment createdPayment = payment.Create(apiContext);

            //if the createdPayment.state is "approved" it means the payment was successful else not

            if (createdPayment != null && createdPayment.state.ToLower() != "approved")
            {
                return true;
            }

            return false;
        }
    }
}