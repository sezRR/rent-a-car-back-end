using Core.Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Business.Constants
{
    public class Messages
    {
        public static string BrandsListed = "Brands Listed";
        public static string BrandAdded = "Brand Added";
        public static string BrandUpdated = "Brand Updated";
        public static string BrandDeleted = "Brand Deleted";
        public static string BrandNameInvalid = "Brand Name is Invalid";

        public static string CarsListed = "Cars Listed";
        public static string CarAdded = "Car Added";
        public static string CarUpdated = "Car Updated";
        public static string CarDeleted = "Car Deleted";
        public static string CarIsInvalid = "Car is Invalid";
        public static string MissingCarsBothFutures = "You need to write a explanatory description about your can be rentable car and You need to write a realistic price for your can be rentable car! (Car Daily Price must be Greater than 0$)";
        public static string MissingCarsDescription = "You need to write a explanatory description about your can be rentable car!";
        public static string MissingCarsPrice = "You need to write a realistic price for your can be rentable car! (Car Daily Price must be Greater than 0$)";
        public static string TheCarNotFound = "Desired car is not on the database";
        public static string CarNotAvailable = "This car already rentaled by someone";

        public static string ColorsListed = "Colors Listed";
        public static string ColorAdded = "Color Added";
        public static string ColorUpdated = "Color Updated";
        public static string ColorDeleted = "Color Deleted";
        public static string ColorNameInvalid = "Color Name is Invalid";

        public static string CustomersListed = "Customers Listed";
        public static string CustomerAdded = "Customer Added";
        public static string CustomerUpdated = "Customer Updated";
        public static string CustomerDeleted = "Customer Deleted";
        public static string CustomerCompanyNameInvalid = "Customer Company Name is Invalid";

        public static string UsersListed = "Users Listed";
        public static string UserAdded = "User Added";
        public static string UserUpdated = "User Updated";
        public static string UserDeleted = "User Deleted";
        public static string UserFirstNameInvalid = "User First Name is Invalid";

        public static string RentalsListed = "Rentals Listed";
        public static string RentalAdded = "Rental Added";
        public static string RentalUpdated = "Rental Updated";
        public static string RentalDeleted = "Rental Deleted";

        public static string CarImageLimitReached = "You can not add more picture about your car. You reached limit of the car images (can only upload 5 images to a car)";

        public static string MaintenanceTime = "Servers in Maintenance (Time)";
        public static string UserNotFound = "User not found";
        public static string PasswordError = "Password is wrong";
        public static string SuccessfulLogin = "Logged in to system successfully";
        public static string UserAlreadyExists = "User already registered";
        public static string UserRegistered = "User registered to the system successfully";
        public static string AccessTokenCreated = "Access Token Created";

        public static string AuthorizationDenied = "You do not have permission";

        public static string PaymentFailed = "Payment failed";
        public static string PaymentSuccessful = "Payment successful";
    }
}
