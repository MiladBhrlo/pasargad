# سناریوهای تست درگاه پرداخت پاسارگاد

## سناریو 1: پرداخت موفق
1. دریافت توکن
   - متد: POST
   - آدرس: `/token/getToken`
   - بدنه:
     ```json
     {
         "Username": "ipgTestUser",
         "Password": "123456"
     }
     ```
   - توکن دریافتی را در متغیر `token` ذخیره کنید

2. درخواست پرداخت
   - متد: POST
   - آدرس: `/api/payment/purchase`
   - هدر: `Authorization: Bearer {{token}}`
   - بدنه:
     ```json
     {
         "ServiceCode": "8",
         "ServiceType": "PURCHASE",
         "PaymentCode": "123456",
         "InvoiceNumber": "INV-001",
         "Amount": 1000,
         "RedirectUrl": "https://your-site.com/callback"
     }
     ```
   - URL دریافتی را در مرورگر باز کنید
   - روی دکمه "پرداخت موفق" کلیک کنید

3. تایید تراکنش
   - متد: POST
   - آدرس: `/api/payment/confirm-transactions`
   - هدر: `Authorization: Bearer {{token}}`
   - بدنه:
     ```json
     {
         "Invoice": "INV-001",
         "TerminalCode": "123456"
     }
     ```

## سناریو 2: پرداخت ناموفق
1. دریافت توکن (مشابه سناریو 1)
2. درخواست پرداخت (مشابه سناریو 1)
3. در صفحه پرداخت روی دکمه "پرداخت ناموفق" کلیک کنید
4. استعلام وضعیت تراکنش
   - متد: POST
   - آدرس: `/api/payment/payment-inquiry`
   - هدر: `Authorization: Bearer {{token}}`
   - بدنه:
     ```json
     {
         "InvoiceId": "INV-001",
         "TerminalCode": "123456"
     }
     ```

## سناریو 3: برگشت تراکنش موفق
1. دریافت توکن (مشابه سناریو 1)
2. درخواست پرداخت (مشابه سناریو 1)
3. پرداخت موفق (مشابه سناریو 1)
4. تایید تراکنش (مشابه سناریو 1)
5. برگشت تراکنش
   - متد: POST
   - آدرس: `/api/payment/reverse-transactions`
   - هدر: `Authorization: Bearer {{token}}`
   - بدنه:
     ```json
     {
         "Invoice": "INV-001",
         "TerminalCode": "123456"
     }
     ```

## سناریو 4: برگشت از خرید
1. دریافت توکن (مشابه سناریو 1)
2. درخواست پرداخت (مشابه سناریو 1)
3. پرداخت موفق (مشابه سناریو 1)
4. تایید تراکنش (مشابه سناریو 1)
5. برگشت از خرید
   - متد: POST
   - آدرس: `/api/payment/refund-transactions`
   - هدر: `Authorization: Bearer {{token}}`
   - بدنه:
     ```json
     {
         "Invoice": "INV-001",
         "TerminalCode": "123456",
         "Type": "RefundOnline",
         "Amount": 1000
     }
     ```

## سناریو 5: تایید رمز کارت
1. دریافت توکن (مشابه سناریو 1)
2. تایید رمز کارت
   - متد: POST
   - آدرس: `/api/payment/inquiry-confirm-pin`
   - هدر: `Authorization: Bearer {{token}}`
   - بدنه:
     ```json
     {
         "CardNumber": "6037701234567890",
         "ExpireMonth": "12",
         "ExpireYear": "99",
         "Cvv2": "1234",
         "Pin2": "123456"
     }
     ```

## سناریو 6: استعلام وضعیت با شماره کارت هش شده
1. دریافت توکن (مشابه سناریو 1)
2. درخواست پرداخت (مشابه سناریو 1)
3. پرداخت موفق (مشابه سناریو 1)
4. استعلام وضعیت با شماره کارت هش شده
   - متد: POST
   - آدرس: `/api/payment/payment-inquiryv2`
   - هدر: `Authorization: Bearer {{token}}`
   - بدنه:
     ```json
     {
         "InvoiceId": "INV-001",
         "TerminalCode": "123456",
         "HashedCardNumber": "ba6567ba6c9fc28e1434b838a91028a4250185fb6dd99d62c0392538a087c8be"
     }
     ```

## سناریو 7: تاییدیه پرداخت با اطلاعات تکمیلی
1. دریافت توکن (مشابه سناریو 1)
2. درخواست پرداخت (مشابه سناریو 1)
3. پرداخت موفق (مشابه سناریو 1)
4. تاییدیه پرداخت با اطلاعات تکمیلی
   - متد: POST
   - آدرس: `/api/payment/verify-payment`
   - هدر: `Authorization: Bearer {{token}}`
   - بدنه:
     ```json
     {
         "Invoice": "INV-001",
         "TerminalCode": "123456"
     }
     ```

## نکات مهم تست
1. قبل از اجرای هر سناریو، مطمئن شوید که توکن معتبر دارید
2. برای هر تراکنش از شماره فاکتور یکتا استفاده کنید
3. مبالغ را متناسب با نیاز خود تنظیم کنید
4. آدرس بازگشت را به آدرس واقعی سایت خود تغییر دهید
5. در صورت نیاز به تست خطاها، مقادیر نامعتبر را امتحان کنید 