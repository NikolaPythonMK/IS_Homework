using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using TicketsSale.Domain.DomainModels;
using TicketsSale.Domain.Identity;
using TicketsSale.Domain.Relations;

namespace TicketsSale.Repository
{
    public class ApplicationDbContext : IdentityDbContext<TicketsSaleApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCards { get; set; }
        public virtual DbSet<TicketInShoppingCart> TicketInShoppingCarts { get; set; }
        public virtual DbSet<EmailMessage> EmailMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureTicketEntity(builder);
            ConfigureTicketInShoppingCartEntity(builder);
            ConfigureShoppingCartEntity(builder);
            ConfigureTicketInOrderEntity(builder);
        }

        private void ConfigureTicketEntity(ModelBuilder builder)
        {
            builder.Entity<Ticket>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();
        }

        private void ConfigureTicketInShoppingCartEntity(ModelBuilder builder)
        {
            builder.Entity<TicketInShoppingCart>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<TicketInShoppingCart>()
                .HasOne(z => z.Ticket)
                .WithMany(z => z.TicketInShoppingCart)
                .HasForeignKey(z => z.TicketId);

            builder.Entity<TicketInShoppingCart>()
                .HasOne(z => z.ShoppingCart)
                .WithMany(z => z.TicketInShoppingCart)
                .HasForeignKey(z => z.ShoppingCartId);
        }

        private void ConfigureShoppingCartEntity(ModelBuilder builder)
        {
            builder.Entity<ShoppingCart>()
                .HasOne<TicketsSaleApplicationUser>(z => z.User)
                .WithOne(z => z.ShoppingCart)
                .HasForeignKey<ShoppingCart>(z => z.UserId);
        }

        private void ConfigureTicketInOrderEntity(ModelBuilder builder)
        {
            builder.Entity<TicketInOrder>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<TicketInOrder>()
                .HasOne(z => z.Ticket)
                .WithMany(z => z.TicketInOrder)
                .HasForeignKey(z => z.TicketId);

            builder.Entity<TicketInOrder>()
                .HasOne(z => z.Order)
                .WithMany(z => z.TicketInOrder)
                .HasForeignKey(z => z.OrderId);
        }
    }
}
