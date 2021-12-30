using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//Slide 58 - 79
using eCommerceWebsite_1912C1.Models; //1st Step
using System.Web.Security;

using PagedList.Mvc;
using PagedList;
using System.Text;

namespace eCommerceWebsite_1912C1.Controllers
{
    public class HomeController : Controller
    {
        //2nd step
        private eCommerce_1912C1Entities db = new eCommerce_1912C1Entities();
             
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User obj)
        {
            if (ModelState.IsValid)
            {
                var isEmailExists = db.Users.Any(m => m.Email == obj.Email);

                if (isEmailExists == true)
                {
                    ModelState.AddModelError("UserVerification", "This email already exists.");
                    return View(obj);
                }

                obj.Registration_Date = DateTime.Today;
                obj.Email_Verified = "Y";
                obj.Role = "Customer";

                db.Users.Add(obj);

                db.SaveChanges();

                TempData["Successful"] = "<script>alert('Your account has been created!');</script>";

                return RedirectToAction("Index");
            }

            return View(obj);
        }


        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(LoginModel objUser)
        {
            if (ModelState.IsValid)
            {
                var obj = db.Users.Where(a => a.Email.Equals(objUser.Email)
                && a.Password.Equals(objUser.Password)
                && a.Email_Verified.Equals("Y")).FirstOrDefault(); //LINQ QUERY

                if (obj != null) //on success
                {
                    Session["UserID"] = obj.User_ID;
                    Session["UserName"] = obj.Name;
                    Session["UserAddress"] = obj.Address;
                    Session["UserPhone"] = obj.Contact_Number;
                    Session["UserEmail"] = obj.Email;

                    FormsAuthentication.SetAuthCookie(obj.Email, false);
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("InvalidLoginError", "Invalid credentials or email not verified!");
            return View();
        }



        public ActionResult Shop(int? page)
        {
            ViewBag.Category_ID = new SelectList(db.Categories,"Category_ID","Category_Name");
            Products_with_Filter model = new Products_with_Filter();
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            model.productList = db.Products.OrderBy(a => a.Product_id).ToPagedList(pageNumber, pageSize);
            return View(model);
        }

        [HttpPost]
        public ActionResult Shop(int? page, decimal min_price, decimal max_price, string Category_ID)
        {
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Category_Name", Category_ID);
            Products_with_Filter model = new Products_with_Filter();
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if(Category_ID == "")
            { 
            model.productList = db.Products.OrderBy(a => a.Product_id)
                    .Where(a => a.Price >= min_price && a.Price <= max_price)
                    .ToPagedList(pageNumber, pageSize);
            }
            else
            {
                int catID = Convert.ToInt32(Category_ID);
                model.productList = db.Products.OrderBy(a => a.Product_id)
                    .Where(a => a.Price >= min_price && a.Price <= max_price && a.Category_ID == catID)
                    .ToPagedList(pageNumber, pageSize);

            }
            return View(model);
        }


        public ActionResult ProductDetails(int? id)
        {
            if (id == null)
            {
                return new HttpNotFoundResult();
            }
            Product obj = db.Products.Find(id);
            if(obj == null)
            {
                return new HttpNotFoundResult();
            }
            return View(obj);
        }


        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }



        public ActionResult AddToCart(int productId, string url)
        {
            if (Session["cart"] == null)
            {

                List<cart> cart = new List<cart>();
                var product = db.Products.Find(productId);
                TempData["SuccessAdded"] = product.Name + " added to cart!";
                cart.Add(new cart()
                {
                    Product = product,
                    qty = 1
                });
                Session["cart"] = cart;
            }
            else
            {

                List<cart> cart = (List<cart>)Session["cart"];
                if (cart.Count() == 0)
                {
                    List<cart> ct = new List<cart>();
                    var product = db.Products.Find(productId);
                    TempData["SuccessAdded"] = product.Name + " added to cart!";
                    ct.Add(new cart()
                    {
                        Product = product,
                        qty = 1
                    });
                    Session["cart"] = ct;
                }
                else
                {
                    var count = cart.Count();
                    var product = db.Products.Find(productId);
                    TempData["SuccessAdded"] = product.Name + " added to cart!";
                    for (int i = 0; i < count; i++)
                    {
                        if (cart[i].Product.Product_id == productId)
                        {
                            int prevQty = cart[i].qty;
                            cart.Remove(cart[i]);
                            cart.Add(new cart()
                            {
                                Product = product,
                                qty = prevQty + 1
                            });

                            break;
                        }
                        else
                        {
                            var prd = cart.Where(x => x.Product.Product_id ==
                           productId).SingleOrDefault();
                            if (prd == null)
                            {
                                cart.Add(new cart()
                                {
                                    Product = product,
                                    qty = 1
                                });
                            }

                        }
                    }
                    Session["cart"] = cart;
                }

            }

            //TempData["SuccessAdded"] = "Successfully added product!";
            return Redirect(url);
        }


        public ActionResult RemoveFromCart(int productId, string url)
        {
            List<cart> cart = (List<cart>)Session["cart"];
            foreach (var item in cart)
            {
                if (item.Product.Product_id == productId)
                {
                    cart.Remove(item);
                    break;
                }
            }
            Session["cart"] = cart;
            return Redirect(url);
        }



        public ActionResult ViewCart()
        {
            return View();
        }


        // Step-2) Create action method for decreasing quantity
        public ActionResult DecreaseQty(int productId)
        {
            if (Session["cart"] != null)
            {
                List<cart> cart = (List<cart>)Session["cart"];
                var product = db.Products.Find(productId);
                foreach (var item in cart)
                {
                    if (item.Product.Product_id == productId)
                    {
                        int prevQty = item.qty;
                        if (prevQty > 0)
                        {
                            cart.Remove(item);
                            cart.Add(new cart()
                            {
                                Product = product,
                                qty = prevQty - 1
                            });
                        }
                        break;
                    }

                }
                Session["cart"] = cart;
            }
            return Redirect("ViewCart");
        }

        // Step-3) Create action method for Checkout page
        public ActionResult CheckOut()
        {
            if (Session["UserName"] == null)
            {
                TempData["UserNotLoggedIn"] = "<script>alert('Please login first.')</script>";
                return RedirectToAction("Login", "Home");
            }
            return View();
        }


        private readonly Random _random = new Random();
        public string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);
            char offset = lowerCase ? 'a' : 'A';
            const int letteroffset = 26;
            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + letteroffset);
                builder.Append(@char);
            }
            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }


        public ActionResult PlaceOrder()
        {
            List<cart> cart = (List<cart>)Session["cart"];
            int totalAmount = 0;
            int totalQty = 0;

            if(ModelState.IsValid)
            {
                string orderCode = RandomString(8);
                Order o1 = new Order();
                o1.Order_Date = DateTime.Now;
                o1.Order_Status = "Pending";
                o1.Payment_Made = 0;
                o1.User_ID = Convert.ToInt32(Session["UserID"]);
                o1.Payment_Method = "COD";
                foreach(var item in cart)
                {
                    totalAmount += Convert.ToInt32(item.qty) * Convert.ToInt32(item.Product.Price);
                    totalQty += Convert.ToInt32(item.qty);

                    Order_Details o2 = new Order_Details();
                    o2.Product_id = item.Product.Product_id;
                    o2.Qty = item.qty;
                    o2.Order_Code = orderCode;
                    o2.Order_ID = o1.Order_ID;
                    db.Order_Details.Add(o2);
                }
                o1.Total_Amount = totalAmount;
                o1.Quantity = totalQty;
                db.Orders.Add(o1);
                db.SaveChanges();
                ModelState.Clear();
                Session["cart"] = null;
            }
            TempData["OrderPlaced"] = "<script>alert('Your order has been placed!');</script>";
            return RedirectToAction("Index");
        }

    }
}