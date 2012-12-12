using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using DotNetLombardia.NancyFxDemo.Api.Models;
using Nancy;
using Nancy.Json;
using Nancy.ModelBinding;

namespace DotNetLombardia.NancyFxDemo.Api.Nancy
{
    public class CategoriesModule : NancyModule
    {
        private NorthwindEntities _db;

        public CategoriesModule(NorthwindEntities context)
            : base("api")
        {
            SetupContext(context);

            //Before += ctx => OAuthChecker.CheckOAuth(ctx);

            Get[@"/Categories"] = r => GetCategories();

            Get[@"/Categories/{id}"] = r => GetCategory(r);

            Put[@"/Categories/{id}"] = r => PutCategory(r);

            Post[@"/Categories"] = r => PostCategory(r);

            Delete[@"/Categories/{id}"] = r => DeleteCategory(r);
        }

        private dynamic GetCategories()
        {
            var query = _db.Categories.OrderBy(p => p.CategoryID).AsQueryable();
            if (Request.Query.nameLike.HasValue)
            {
                string name=((string)Request.Query.nameLike);
                query = query.Where(p => p.CategoryName.Contains(name));
            }
            if (Request.Query.Offset.HasValue) query = query.Skip((int) Request.Query.Offset);
            if (Request.Query.Limit.HasValue) query = query.Take((int) Request.Query.Limit);
            return query;
        }


        private dynamic DeleteCategory(dynamic r)
        {
            var category = _db.Categories.Find((int)r.id);
            if (category == null)
            {
                return HttpStatusCode.NotFound;
            }

            _db.Categories.Remove(category);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return HttpStatusCode.NotFound;
            }

            return category;
        }

        private dynamic PostCategory(dynamic r)
        {
            var category = this.Bind<Categories>();

            if (category.CategoryID > 0)
            {
                _db.Categories.Add(category);
                _db.SaveChanges();
                
                return HttpStatusCode.Created;
            }
            return HttpStatusCode.BadRequest;
        }

        private dynamic PutCategory(dynamic r)
        {
            var category = this.Bind<Categories>();
            if ((int)r.id == category.CategoryID)
            {
                _db.Entry(category).State = EntityState.Modified;

                try
                {
                    _db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return HttpStatusCode.NotFound;
                }

                return HttpStatusCode.OK;
            }
            return HttpStatusCode.BadRequest;
        }

        private dynamic GetCategory(dynamic r)
        {
            var category = _db.Categories.Find((int)r.id);
            if (category == null)
            {
                return HttpStatusCode.NotFound;
            }
            return category;
        }

        private void SetupContext(NorthwindEntities context)
        {
            _db = context;
            _db.Configuration.ProxyCreationEnabled = false;
            JsonSettings.MaxJsonLength = Int32.MaxValue;
        }
    }
}