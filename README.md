<br>
<br>
<br>
 
<p align="center">
 <img width="100px" src="https://www.seekpng.com/png/full/419-4199738_final-product-image-isometric-car-illustration-png.png" align="center" alt="GitHub Readme Stats" />
 <h2 align="center">RENTAR (RENT CAR) - BACKEND</h2>

 <p align="center">Rentar 'Backend' project with C# - ASP.NET Core</p>
 <p align="center">Click for <a href="https://github.com/sezRR/rent-a-car-front-end">The Frontend</a></p>
</p>



<br>
<br>
<br>
<br>
<br>



# RENTAR BACKEND

<br>

**About the Project Briefly**
1. Backend is appropriate for SOLID 
2. All entities have CRUD operations from EntityRepository 
3. For some necessaries, DTO objects used
4. There is Password Security Feature
5. There is Token Feature
6. There is IoC Feature

<br><br>

**Entity Repository Operations** (Core)
- Add
- Update
- Delete
- Get (with filter)
- GetAll (optional filter)

<br><br>

**Entities**
- IEntity (Core)
  - Brand
  - CarImage
  - Car
  - Color
  - Customer
  - Findeks
  - Rental
  - User
  - Payment
  - OperationClaims (Core)
  - User (Core)
  - UserOperationClaims (Core)
  
 <br>
 
- IDto (Core)
  - CarDetailDto
  - RentalDetailDto
  - UserForLoginDto
  - UserForRegisterDto
  - UserForUpdateDto

<br><br>

**Result** (Core)
- IResult
  - IResult
    - Result
      - SuccessResult
      - Error Result
    
  - IDataResult
    - DataResult
      - SuccessDataResult
      - ErrorDataResult
      
<br><br>

**Aspects (used Autofac)** (Core)
- Caching
- Performance
- Transaction
- Validation

<br><br>

**Security** (Core)
- Encryption
  - SecurityKeyHelper
  - SigningCredentialsHelper

<br>

- Hashing
  - HashingHelper

<br>

- JWT
  - TokenOptions (Entity)
  - AccessToken (Entity)
  - ITokenHelper
    - JWTHelper

<br><br>

_Result implementation for healthy checking HTTP Request status_

_On WebApi used wwwroot and you can easily upload your images to project(root)_

**Dependency Resolver**: _Autofac_

**Validation**: _FluentValidation_

**Database Operations & Data Access Operations**: _EntityFrameworkCore_
