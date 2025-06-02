// سناریو 1: فرآیند کامل پرداخت موفق
pm.test("سناریو 1: فرآیند کامل پرداخت موفق", function () {
    // 1. دریافت توکن
    pm.sendRequest({
        url: pm.variables.get("baseUrl") + "/token/getToken",
        method: "POST",
        header: {
            "Content-Type": "application/json"
        },
        body: {
            mode: "raw",
            raw: JSON.stringify({
                Username: "ipgTestUser",
                Password: "123456"
            })
        }
    }, function (err, res) {
        pm.expect(err).to.be.null;
        pm.expect(res.code).to.equal(200);
        pm.expect(res.json().ResultCode).to.equal(0);
        pm.expect(res.json().Token).to.exist;
        pm.variables.set("token", res.json().Token);

        // 2. درخواست پرداخت
        pm.sendRequest({
            url: pm.variables.get("baseUrl") + "/api/payment/purchase",
            method: "POST",
            header: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + pm.variables.get("token")
            },
            body: {
                mode: "raw",
                raw: JSON.stringify({
                    ServiceCode: "8",
                    ServiceType: "PURCHASE",
                    PaymentCode: "123456",
                    Invoice: "INV-001",
                    Amount: 1000,
                    CallbackApi: "https://your-site.com/callback",
                    TerminalNumber: 123456
                })
            }
        }, function (err, res) {
            pm.expect(err).to.be.null;
            pm.expect(res.code).to.equal(200);
            pm.expect(res.json().ResultCode).to.equal(0);
            pm.expect(res.json().Data.UrlId).to.exist;
            pm.expect(res.json().Data.Url).to.exist;
            pm.variables.set("urlId", res.json().Data.UrlId);
            pm.variables.set("paymentUrl", res.json().Data.Url);

            // 3. تایید تراکنش
            pm.sendRequest({
                url: pm.variables.get("baseUrl") + "/api/payment/confirm-transactions",
                method: "POST",
                header: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + pm.variables.get("token")
                },
                body: {
                    mode: "raw",
                    raw: JSON.stringify({
                        Invoice: "INV-001",
                        UrlId: pm.variables.get("urlId")
                    })
                }
            }, function (err, res) {
                pm.expect(err).to.be.null;
                pm.expect(res.code).to.equal(200);
                pm.expect(res.json().ResultCode).to.equal(0);
                pm.expect(res.json().Data.ReferenceNumber).to.exist;
                pm.expect(res.json().Data.TrackId).to.exist;
                pm.variables.set("referenceNumber", res.json().Data.ReferenceNumber);
                pm.variables.set("trackId", res.json().Data.TrackId);
            });
        });
    });
});

// سناریو 2: فرآیند پرداخت با PEP
pm.test("سناریو 2: فرآیند پرداخت با PEP", function () {
    // 1. درخواست پرداخت PEP
    pm.sendRequest({
        url: pm.variables.get("baseUrl") + "/pep/api/payment/request",
        method: "POST",
        header: {
            "Content-Type": "application/json"
        },
        body: {
            mode: "raw",
            raw: JSON.stringify({
                InvoiceNumber: "INV-002",
                Amount: 2000,
                TerminalCode: "123456",
                RedirectUrl: "https://your-site.com/callback"
            })
        }
    }, function (err, res) {
        pm.expect(err).to.be.null;
        pm.expect(res.code).to.equal(200);
        pm.expect(res.json().IsSuccess).to.be.true;
        pm.expect(res.json().Token).to.exist;
        pm.variables.set("pepToken", res.json().Token);

        // 2. تایید پرداخت PEP
        pm.sendRequest({
            url: pm.variables.get("baseUrl") + "/pep/api/payment/verify",
            method: "POST",
            header: {
                "Content-Type": "application/json"
            },
            body: {
                mode: "raw",
                raw: JSON.stringify({
                    InvoiceNumber: "INV-002",
                    Amount: 2000,
                    TerminalCode: "123456",
                    RedirectUrl: "https://your-site.com/callback"
                })
            }
        }, function (err, res) {
            pm.expect(err).to.be.null;
            pm.expect(res.code).to.equal(200);
            pm.expect(res.json().IsSuccess).to.be.true;
            pm.expect(res.json().Token).to.exist;
        });
    });
});

// سناریو 3: استعلام وضعیت تراکنش
pm.test("سناریو 3: استعلام وضعیت تراکنش", function () {
    // 1. دریافت توکن
    pm.sendRequest({
        url: pm.variables.get("baseUrl") + "/token/getToken",
        method: "POST",
        header: {
            "Content-Type": "application/json"
        },
        body: {
            mode: "raw",
            raw: JSON.stringify({
                Username: "ipgTestUser",
                Password: "123456"
            })
        }
    }, function (err, res) {
        pm.expect(err).to.be.null;
        pm.expect(res.code).to.equal(200);
        pm.expect(res.json().ResultCode).to.equal(0);
        pm.expect(res.json().Token).to.exist;
        pm.variables.set("token", res.json().Token);

        // 2. استعلام وضعیت تراکنش
        pm.sendRequest({
            url: pm.variables.get("baseUrl") + "/api/payment/payment-inquiry",
            method: "POST",
            header: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + pm.variables.get("token")
            },
            body: {
                mode: "raw",
                raw: JSON.stringify({
                    InvoiceId: "INV-001",
                    TerminalCode: "123456"
                })
            }
        }, function (err, res) {
            pm.expect(err).to.be.null;
            pm.expect(res.code).to.equal(200);
            pm.expect(res.json().ResultCode).to.equal(0);
            pm.expect(res.json().Data.Status).to.equal(13033);
            pm.expect(res.json().Data.TrackId).to.exist;
            pm.expect(res.json().Data.TransactionId).to.exist;
        });
    });
});

// سناریو 4: برگشت تراکنش
pm.test("سناریو 4: برگشت تراکنش", function () {
    // 1. دریافت توکن
    pm.sendRequest({
        url: pm.variables.get("baseUrl") + "/token/getToken",
        method: "POST",
        header: {
            "Content-Type": "application/json"
        },
        body: {
            mode: "raw",
            raw: JSON.stringify({
                Username: "ipgTestUser",
                Password: "123456"
            })
        }
    }, function (err, res) {
        pm.expect(err).to.be.null;
        pm.expect(res.code).to.equal(200);
        pm.expect(res.json().ResultCode).to.equal(0);
        pm.expect(res.json().Token).to.exist;
        pm.variables.set("token", res.json().Token);

        // 2. برگشت تراکنش
        pm.sendRequest({
            url: pm.variables.get("baseUrl") + "/api/payment/reverse-transactions",
            method: "POST",
            header: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + pm.variables.get("token")
            },
            body: {
                mode: "raw",
                raw: JSON.stringify({
                    Invoice: "INV-001",
                    TerminalCode: "123456"
                })
            }
        }, function (err, res) {
            pm.expect(err).to.be.null;
            pm.expect(res.code).to.equal(200);
            pm.expect(res.json().ResultCode).to.equal(0);
            pm.expect(res.json().ResultMsg).to.equal("Successful");
        });
    });
});

// سناریو 5: تایید رمز کارت
pm.test("سناریو 5: تایید رمز کارت", function () {
    // 1. دریافت توکن
    pm.sendRequest({
        url: pm.variables.get("baseUrl") + "/token/getToken",
        method: "POST",
        header: {
            "Content-Type": "application/json"
        },
        body: {
            mode: "raw",
            raw: JSON.stringify({
                Username: "ipgTestUser",
                Password: "123456"
            })
        }
    }, function (err, res) {
        pm.expect(err).to.be.null;
        pm.expect(res.code).to.equal(200);
        pm.expect(res.json().ResultCode).to.equal(0);
        pm.expect(res.json().Token).to.exist;
        pm.variables.set("token", res.json().Token);

        // 2. تایید رمز کارت
        pm.sendRequest({
            url: pm.variables.get("baseUrl") + "/api/payment/inquiry-confirm-pin",
            method: "POST",
            header: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + pm.variables.get("token")
            },
            body: {
                mode: "raw",
                raw: JSON.stringify({
                    CardNumber: "6037701234567890",
                    ExpireMonth: "12",
                    ExpireYear: "99",
                    Cvv2: "1234",
                    Pin2: "123456"
                })
            }
        }, function (err, res) {
            pm.expect(err).to.be.null;
            pm.expect(res.code).to.equal(200);
            pm.expect(res.json().ResultCode).to.equal(0);
            pm.expect(res.json().ResultMsg).to.equal("Successful");
        });
    });
}); 