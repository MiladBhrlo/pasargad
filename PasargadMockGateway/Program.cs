using PasargadMockGateway.Models;
using PasargadMockGateway.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pasargad Mock Gateway API", Version = "v1" });
    
    // اضافه کردن پشتیبانی از JWT در Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// اضافه کردن تنظیمات JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "YourSecretKeyHere"))
        };
    });

// اضافه کردن سرویس تراکنش
builder.Services.AddSingleton<TransactionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();
app.MapRazorPages();
app.MapControllers();

// درخواست پرداخت
app.MapPost("/pep/api/payment/request", (PaymentRequest request) =>
{
    // در محیط واقعی، اینجا باید امضای درخواست را بررسی کنیم
    var response = new PaymentResponse
    {
        IsSuccess = true,
        Message = "درخواست با موفقیت ثبت شد",
        Token = Guid.NewGuid().ToString("N")
    };
    
    return Results.Ok(response);
})
.WithName("PaymentRequest")
.WithOpenApi();

// بررسی وضعیت پرداخت
app.MapPost("/pep/api/payment/verify", (PaymentRequest request) =>
{
    var response = new PaymentResponse
    {
        IsSuccess = true,
        Message = "پرداخت با موفقیت انجام شد",
        Token = request.InvoiceNumber
    };
    
    return Results.Ok(response);
})
.WithName("PaymentVerify")
.WithOpenApi();

// بازگشت از درگاه پرداخت
app.MapGet("/pep/api/payment/callback", (string token, string invoiceNumber, string terminalCode) =>
{
    return Results.Redirect($"/payment/callback?token={token}&invoiceNumber={invoiceNumber}&terminalCode={terminalCode}");
})
.WithName("PaymentCallback")
.WithOpenApi();

// سرویس دریافت توکن
app.MapPost("/token/getToken", (TokenRequest request) =>
{
    // در محیط واقعی، اینجا باید اعتبارسنجی نام کاربری و رمز عبور انجام شود
    if (request.Username == "ipgTestUser" && request.Password == "123456")
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? "YourSecretKeyHere");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.Role, "merchant")
            }),
            Expires = DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var response = new TokenResponse
        {
            ResultMsg = ErrorCodes.GetPersianMessage(0),
            ResultCode = 0,
            Token = tokenHandler.WriteToken(token),
            Username = request.Username,
            FirstName = "ipgTestUserFirst",
            LastName = "ipgTestUserLast",
            UserId = "10060",
            Roles = new List<Role>
            {
                new Role { Authority = "merchant" }
            }
        };

        return Results.Ok(response);
    }

    return Results.BadRequest(new TokenResponse
    {
        ResultMsg = ErrorCodes.GetPersianMessage(401),
        ResultCode = 401
    });
})
.WithName("GetToken")
.WithOpenApi();

// سرویس خرید و خرید شناسه دار
app.MapPost("/api/payment/purchase", [Authorize] (PurchaseRequest request, TransactionService transactionService) =>
{
    // در محیط واقعی، اینجا باید اعتبارسنجی‌های لازم انجام شود
    if (request.ServiceCode != "8" || request.ServiceType != "PURCHASE")
    {
        return Results.BadRequest(new PurchaseResponse
        {
            ResultMsg = ErrorCodes.GetPersianMessage(13000),
            ResultCode = 13000
        });
    }

    // اعتبارسنجی PaymentCode برای خرید شناسه دار
    if (string.IsNullOrEmpty(request.PaymentCode))
    {
        return Results.BadRequest(new PurchaseResponse
        {
            ResultMsg = ErrorCodes.GetPersianMessage(13000),
            ResultCode = 13000
        });
    }

    // تولید یک شناسه یکتا برای URL
    var urlId = Guid.NewGuid().ToString("N");
    
    // ذخیره اطلاعات تراکنش
    transactionService.AddTransaction(urlId, new TransactionInfo
    {
        PaymentCode = request.PaymentCode,
        Invoice = request.Invoice,
        TerminalNumber = request.TerminalNumber.ToString(),
        Amount = request.Amount,
        CallbackApi = request.CallbackApi
    });
    
    // ساخت URL درگاه پرداخت
    var baseUrl = builder.Configuration["BaseUrl"] ?? "http://localhost:4000";
    var paymentUrl = $"{baseUrl}/{urlId}";

    var response = new PurchaseResponse
    {
        ResultMsg = ErrorCodes.GetPersianMessage(0),
        ResultCode = 0,
        Data = new PurchaseData
        {
            UrlId = urlId,
            Url = paymentUrl
        }
    };

    return Results.Ok(response);
})
.WithName("Purchase")
.WithOpenApi();

// صفحه درگاه پرداخت
app.MapGet("/{urlId}", (string urlId, TransactionService transactionService) =>
{
    var transaction = transactionService.GetTransaction(urlId);
    if (transaction == null)
    {
        return Results.NotFound();
    }

    return Results.Redirect($"/Payment?token={transaction.PaymentCode}&invoiceNumber={transaction.Invoice}&terminalCode={transaction.TerminalNumber}&amount={transaction.Amount}&redirectUrl={transaction.CallbackApi}");
})
.WithName("PaymentGateway")
.WithOpenApi();

// سرویس تایید تراکنش
app.MapPost("/api/payment/confirm-transactions", [Authorize] (ConfirmRequest request) =>
{
    // در محیط واقعی، اینجا باید اعتبارسنجی‌های لازم انجام شود
    // مثلاً بررسی اینکه آیا تراکنش قبلاً تایید شده است یا خیر
    
    var response = new ConfirmResponse
    {
        ResultMsg = ErrorCodes.GetPersianMessage(0),
        ResultCode = 0,
        Data = new ConfirmData
        {
            Invoice = request.Invoice,
            ReferenceNumber = "141010031417",
            TrackId = "9",
            MaskedCardNumber = "603770******0340",
            HashedCardNumber = "ba6567ba6c9fc28e1434b838a91028a4250185fb6dd99d62c0392538a087c8be",
            RequestDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff"),
            Amount = 1000 // در محیط واقعی، این مقدار باید از دیتابیس خوانده شود
        }
    };

    return Results.Ok(response);
})
.WithName("ConfirmTransaction")
.WithOpenApi();

// سرویس تاییدیه تراکنش
app.MapPost("/api/payment/verify-transactions", [Authorize] (VerifyRequest request) =>
{
    // در محیط واقعی، اینجا باید اعتبارسنجی‌های لازم انجام شود
    // مثلاً بررسی اینکه آیا تراکنش قبلاً تایید شده است یا خیر
    
    var response = new VerifyResponse
    {
        Data = ErrorCodes.GetPersianMessage(0),
        ResultMsg = ErrorCodes.GetPersianMessage(0),
        ResultCode = 0
    };

    return Results.Ok(response);
})
.WithName("VerifyTransaction")
.WithOpenApi();

// سرویس تاییدیه پرداخت با اطلاعات تکمیلی
app.MapPost("/api/payment/verify-payment", [Authorize] (VerifyPaymentRequest request) =>
{
    // در محیط واقعی، اینجا باید اعتبارسنجی‌های لازم انجام شود
    // مثلاً بررسی اینکه آیا تراکنش قبلاً تایید شده است یا خیر
    
    var response = new VerifyPaymentResponse
    {
        ResultMsg = "Successful",
        ResultCode = 0,
        Data = new VerifyPaymentData
        {
            Invoice = request.Invoice,
            ReferenceNumber = "141010031419",
            TrackId = "11",
            MaskedCardNumber = "603770******0340",
            HashedCardNumber = "ba6567ba6c9fc28e1434b838a91028a4250185fb6dd99d62c0392538a087c8be",
            RequestDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff"),
            Amount = 1000 // در محیط واقعی، این مقدار باید از دیتابیس خوانده شود
        }
    };

    return Results.Ok(response);
})
.WithName("VerifyPayment")
.WithOpenApi();

// سرویس برگشت تراکنش
app.MapPost("/api/payment/reverse-transactions", [Authorize] (ReverseRequest request) =>
{
    // در محیط واقعی، اینجا باید اعتبارسنجی‌های لازم انجام شود
    // مثلاً بررسی اینکه آیا تراکنش قبلاً تایید شده است یا خیر
    // و اینکه آیا تراکنش قبلاً برگشت شده است یا خیر
    
    var response = new ReverseResponse
    {
        ResultMsg = "Successful",
        ResultCode = 0
    };

    return Results.Ok(response);
})
.WithName("ReverseTransaction")
.WithOpenApi();

// سرویس callback
app.MapGet("/api/payment/callback", (string invoiceId, string status, string referenceNumber, string trackId) =>
{
    // در محیط واقعی، اینجا باید اعتبارسنجی‌های لازم انجام شود
    // مثلاً بررسی اینکه آیا تراکنش معتبر است یا خیر
    
    var response = new CallbackResponse
    {
        InvoiceId = invoiceId,
        Status = status,
        ReferenceNumber = referenceNumber,
        TrackId = trackId
    };

    // در محیط واقعی، اینجا باید کاربر را به صفحه مناسب هدایت کنیم
    // مثلاً صفحه موفقیت یا شکست پرداخت
    return Results.Ok(response);
})
.WithName("Callback")
.WithOpenApi();

// سرویس استعلام وضعیت تراکنش
app.MapPost("/api/payment/payment-inquiry", [Authorize] (PaymentInquiryRequest request) =>
{
    // در محیط واقعی، اینجا باید اعتبارسنجی‌های لازم انجام شود
    // مثلاً بررسی اینکه آیا تراکنش معتبر است یا خیر
    
    // وضعیت‌های ممکن برای تراکنش:
    // 0: success - تراکنش موفق است
    // 13016: transaction not found - تراکنشی یافت نشد
    // 13022: transaction is not payed - تراکنش پرداخت نشده است
    // 13030: transaction is before verified and it's failed - تراکنش تایید شده و ناموفق است
    // 13031: transaction is not yet verified - تراکنش منتظر تایید است
    // 13033: transaction is verified - تراکنش تایید شده است
    
    var response = new PaymentInquiryResponse
    {
        ResultMsg = "Successful",
        ResultCode = 0,
        Data = new PaymentInquiryData
        {
            Status = 13033, // تراکنش تایید شده است
            TrackId = "12",
            TransactionId = "50333",
            Amount = 1000,
            CardNumber = "603770******0340",
            Invoice = request.InvoiceId,
            Url = "f2560bf76dfef9c51af9767e975dc02a739fbbd7000000006217318",
            ReferenceNumber = "141010031420",
            RequestDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff")
        }
    };

    return Results.Ok(response);
})
.WithName("PaymentInquiry")
.WithOpenApi();

// سرویس استعلام وضعیت تراکنش با شماره کارت هش شده
app.MapPost("/api/payment/payment-inquiryv2", [Authorize] (PaymentInquiryV2Request request) =>
{
    // در محیط واقعی، اینجا باید اعتبارسنجی‌های لازم انجام شود
    // مثلاً بررسی اینکه آیا تراکنش معتبر است یا خیر
    
    // وضعیت‌های ممکن برای تراکنش:
    // 0: success - تراکنش موفق است
    // 13016: transaction not found - تراکنشی یافت نشد
    // 13022: transaction is not payed - تراکنش پرداخت نشده است
    // 13030: transaction is before verified and it's failed - تراکنش تایید شده و ناموفق است
    // 13031: transaction is not yet verified - تراکنش منتظر تایید است
    // 13029: transaction is verified - تراکنش تایید شده است
    
    var response = new PaymentInquiryV2Response
    {
        ResultMsg = "Successful",
        ResultCode = 0,
        Data = new PaymentInquiryV2Data
        {
            Status = 13029, // تراکنش تایید شده است
            TrackId = "10",
            TransactionId = "43",
            Amount = 10000,
            CardNumber = "603770******0340",
            Invoice = request.InvoiceId,
            Url = "7c5e1df234b7021a59a0e6d2ac373c5cb0697d31000000007461803",
            ReferenceNumber = "141010000008",
            RequestDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff"),
            HashedCardNumber = "ba6567ba6c9fc28e1434b838a91028a4250185fb6dd99d62c0392538a087c8be"
        }
    };

    return Results.Ok(response);
})
.WithName("PaymentInquiryV2")
.WithOpenApi();

// سرویس برگشت از خرید
app.MapPost("/api/payment/refund-transactions", [Authorize] (RefundRequest request) =>
{
    // در محیط واقعی، اینجا باید اعتبارسنجی‌های لازم انجام شود
    // مثلاً بررسی اینکه آیا تراکنش معتبر است یا خیر
    // و اینکه آیا تراکنش قبلاً تایید شده است یا خیر
    // و اینکه آیا مبلغ برگشتی از مبلغ تراکنش اصلی کمتر یا برابر است یا خیر
    
    // اعتبارسنجی نوع تراکنش
    if (request.Type != "RefundOnline" && request.Type != "RefundOffline")
    {
        return Results.BadRequest(new RefundResponse
        {
            ResultMsg = "Invalid transaction type",
            ResultCode = 1
        });
    }
    
    var response = new RefundResponse
    {
        ResultMsg = "Successful",
        ResultCode = 0
    };

    return Results.Ok(response);
})
.WithName("RefundTransaction")
.WithOpenApi();

// سرویس تایید رمز
app.MapPost("/api/payment/inquiry-confirm-pin", [Authorize] (ConfirmPinRequest request) =>
{
    // در محیط واقعی، اینجا باید اعتبارسنجی‌های لازم انجام شود
    // مثلاً بررسی اینکه آیا کارت معتبر است یا خیر
    // و اینکه آیا رمز دوم کارت صحیح است یا خیر
    
    // اعتبارسنجی فرمت ماه و سال انقضا
    if (!int.TryParse(request.ExpireMonth, out int month) || month < 1 || month > 12)
    {
        return Results.BadRequest(new ConfirmPinResponse
        {
            ResultMsg = "Invalid expire month",
            ResultCode = 1
        });
    }
    
    if (!int.TryParse(request.ExpireYear, out int year) || year < 0 || year > 99)
    {
        return Results.BadRequest(new ConfirmPinResponse
        {
            ResultMsg = "Invalid expire year",
            ResultCode = 1
        });
    }
    
    // اعتبارسنجی CVV2
    if (request.Cvv2.Length != 4)
    {
        return Results.BadRequest(new ConfirmPinResponse
        {
            ResultMsg = "Invalid CVV2",
            ResultCode = 1
        });
    }
    
    // اعتبارسنجی رمز دوم
    if (request.Pin2.Length != 6)
    {
        return Results.BadRequest(new ConfirmPinResponse
        {
            ResultMsg = "Invalid PIN2",
            ResultCode = 1
        });
    }
    
    var response = new ConfirmPinResponse
    {
        ResultMsg = "Successful",
        ResultCode = 0
    };

    return Results.Ok(response);
})
.WithName("ConfirmPin")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
