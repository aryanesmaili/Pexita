﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pexita.Data;

#nullable disable

namespace Pexita.Migrations
{
    [DbContext(typeof(AppDBContext))]
    [Migration("20240809220830_sc1")]
    partial class sc1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Pexita.Data.Entities.Authentication.BrandRefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BrandID")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("Revoked")
                        .HasColumnType("datetime2");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BrandID");

                    b.ToTable("BrandRefreshTokens");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Authentication.UserRefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("Revoked")
                        .HasColumnType("datetime2");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserRefreshTokens");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Brands.BrandModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("BrandPicURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResetPasswordCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Brands");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Brands.BrandOrder", b =>
                {
                    b.Property<int>("BrandID")
                        .HasColumnType("int");

                    b.Property<int>("OrderID")
                        .HasColumnType("int");

                    b.HasKey("BrandID", "OrderID");

                    b.HasIndex("OrderID");

                    b.ToTable("BrandOrder");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Comments.CommentsModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProductID")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("ProductID");

                    b.HasIndex("UserID");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Newsletter.BrandNewsletterModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("BrandID")
                        .HasColumnType("int");

                    b.Property<DateTime>("SubscribedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("BrandID");

                    b.HasIndex("UserID");

                    b.ToTable("BrandNewsletters");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Newsletter.ProductNewsLetterModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int?>("BrandModelID")
                        .HasColumnType("int");

                    b.Property<int>("ProductID")
                        .HasColumnType("int");

                    b.Property<DateTime>("SubscribedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("BrandModelID");

                    b.HasIndex("ProductID");

                    b.HasIndex("UserID");

                    b.ToTable("ProductNewsletters");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Orders.OrdersModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<DateTime>("DateIssued")
                        .HasColumnType("datetime2");

                    b.Property<int>("PaymentID")
                        .HasColumnType("int");

                    b.Property<int>("ShoppingCartID")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("PaymentID");

                    b.HasIndex("ShoppingCartID")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Payment.PaymentModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<long>("Amount")
                        .HasColumnType("bigint");

                    b.Property<string>("CardNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateIssued")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateTimePaid")
                        .HasColumnType("datetime2");

                    b.Property<string>("HashedCardNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IDPayTrackID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentOrderID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PaymentVerificationDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ShoppingCartID")
                        .HasColumnType("int");

                    b.Property<bool?>("Successfull")
                        .HasColumnType("bit");

                    b.Property<string>("TransactionID")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("ShoppingCartID");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Products.ProductModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("BrandID")
                        .HasColumnType("int");

                    b.Property<string>("Colors")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<double?>("Price")
                        .HasColumnType("float");

                    b.Property<string>("ProductPicsURL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("BrandID");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Products.ProductRating", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("ProductID")
                        .HasColumnType("int");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("ProductID");

                    b.ToTable("ProductRating");
                });

            modelBuilder.Entity("Pexita.Data.Entities.ShoppingCart.CartItems", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<int?>("ProductID")
                        .HasColumnType("int");

                    b.Property<int>("ShoppingCartID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("ProductID");

                    b.HasIndex("ShoppingCartID");

                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("Pexita.Data.Entities.ShoppingCart.ShoppingCartModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<double>("TotalPrice")
                        .HasColumnType("float");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("ShoppingCarts");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Tags.TagModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("TimesUsed")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Pexita.Data.Entities.User.Address", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Province")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("Pexita.Data.Entities.User.UserModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResetPasswordCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ProductModelTagModel", b =>
                {
                    b.Property<int>("ProductsID")
                        .HasColumnType("int");

                    b.Property<int>("TagsID")
                        .HasColumnType("int");

                    b.HasKey("ProductsID", "TagsID");

                    b.HasIndex("TagsID");

                    b.ToTable("ProductModelTagModel");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Authentication.BrandRefreshToken", b =>
                {
                    b.HasOne("Pexita.Data.Entities.Brands.BrandModel", "Brand")
                        .WithMany("BrandRefreshTokens")
                        .HasForeignKey("BrandID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Authentication.UserRefreshToken", b =>
                {
                    b.HasOne("Pexita.Data.Entities.User.UserModel", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Brands.BrandOrder", b =>
                {
                    b.HasOne("Pexita.Data.Entities.Brands.BrandModel", "Brand")
                        .WithMany("BrandOrders")
                        .HasForeignKey("BrandID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Pexita.Data.Entities.Orders.OrdersModel", "Order")
                        .WithMany("BrandOrders")
                        .HasForeignKey("OrderID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Comments.CommentsModel", b =>
                {
                    b.HasOne("Pexita.Data.Entities.Products.ProductModel", "Product")
                        .WithMany("Comments")
                        .HasForeignKey("ProductID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pexita.Data.Entities.User.UserModel", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Newsletter.BrandNewsletterModel", b =>
                {
                    b.HasOne("Pexita.Data.Entities.Brands.BrandModel", "Brand")
                        .WithMany("BrandNewsLetters")
                        .HasForeignKey("BrandID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pexita.Data.Entities.User.UserModel", "User")
                        .WithMany("BrandNewsletters")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Newsletter.ProductNewsLetterModel", b =>
                {
                    b.HasOne("Pexita.Data.Entities.Brands.BrandModel", null)
                        .WithMany("ProductNewsLetters")
                        .HasForeignKey("BrandModelID");

                    b.HasOne("Pexita.Data.Entities.Products.ProductModel", "Product")
                        .WithMany("NewsLetters")
                        .HasForeignKey("ProductID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pexita.Data.Entities.User.UserModel", "User")
                        .WithMany("ProductNewsletters")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Orders.OrdersModel", b =>
                {
                    b.HasOne("Pexita.Data.Entities.Payment.PaymentModel", "Payment")
                        .WithMany()
                        .HasForeignKey("PaymentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pexita.Data.Entities.ShoppingCart.ShoppingCartModel", "ShoppingCart")
                        .WithOne("Order")
                        .HasForeignKey("Pexita.Data.Entities.Orders.OrdersModel", "ShoppingCartID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pexita.Data.Entities.User.UserModel", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Payment");

                    b.Navigation("ShoppingCart");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Payment.PaymentModel", b =>
                {
                    b.HasOne("Pexita.Data.Entities.ShoppingCart.ShoppingCartModel", "ShoppingCart")
                        .WithMany("Payments")
                        .HasForeignKey("ShoppingCartID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("ShoppingCart");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Products.ProductModel", b =>
                {
                    b.HasOne("Pexita.Data.Entities.Brands.BrandModel", "Brand")
                        .WithMany("Products")
                        .HasForeignKey("BrandID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Products.ProductRating", b =>
                {
                    b.HasOne("Pexita.Data.Entities.Products.ProductModel", "Product")
                        .WithMany("Rating")
                        .HasForeignKey("ProductID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Pexita.Data.Entities.ShoppingCart.CartItems", b =>
                {
                    b.HasOne("Pexita.Data.Entities.Products.ProductModel", "Product")
                        .WithMany("CartItems")
                        .HasForeignKey("ProductID");

                    b.HasOne("Pexita.Data.Entities.ShoppingCart.ShoppingCartModel", "ShoppingCart")
                        .WithMany("CartItems")
                        .HasForeignKey("ShoppingCartID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("ShoppingCart");
                });

            modelBuilder.Entity("Pexita.Data.Entities.ShoppingCart.ShoppingCartModel", b =>
                {
                    b.HasOne("Pexita.Data.Entities.User.UserModel", "User")
                        .WithMany("ShoppingCarts")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Pexita.Data.Entities.User.Address", b =>
                {
                    b.HasOne("Pexita.Data.Entities.User.UserModel", "User")
                        .WithMany("Addresses")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ProductModelTagModel", b =>
                {
                    b.HasOne("Pexita.Data.Entities.Products.ProductModel", null)
                        .WithMany()
                        .HasForeignKey("ProductsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pexita.Data.Entities.Tags.TagModel", null)
                        .WithMany()
                        .HasForeignKey("TagsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Pexita.Data.Entities.Brands.BrandModel", b =>
                {
                    b.Navigation("BrandNewsLetters");

                    b.Navigation("BrandOrders");

                    b.Navigation("BrandRefreshTokens");

                    b.Navigation("ProductNewsLetters");

                    b.Navigation("Products");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Orders.OrdersModel", b =>
                {
                    b.Navigation("BrandOrders");
                });

            modelBuilder.Entity("Pexita.Data.Entities.Products.ProductModel", b =>
                {
                    b.Navigation("CartItems");

                    b.Navigation("Comments");

                    b.Navigation("NewsLetters");

                    b.Navigation("Rating");
                });

            modelBuilder.Entity("Pexita.Data.Entities.ShoppingCart.ShoppingCartModel", b =>
                {
                    b.Navigation("CartItems");

                    b.Navigation("Order")
                        .IsRequired();

                    b.Navigation("Payments");
                });

            modelBuilder.Entity("Pexita.Data.Entities.User.UserModel", b =>
                {
                    b.Navigation("Addresses");

                    b.Navigation("BrandNewsletters");

                    b.Navigation("Comments");

                    b.Navigation("Orders");

                    b.Navigation("ProductNewsletters");

                    b.Navigation("RefreshTokens");

                    b.Navigation("ShoppingCarts");
                });
#pragma warning restore 612, 618
        }
    }
}
