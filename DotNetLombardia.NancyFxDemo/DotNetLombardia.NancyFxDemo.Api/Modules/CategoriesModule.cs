using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using DotNetLombardia.NancyFxDemo.Api.Models;
using Nancy;
using Nancy.Json;
using Nancy.ModelBinding;

namespace DotNetLombardia.NancyFxDemo.Api.Modules
{
    public class CategoriesModule : NancyModule
    {
        private NorthwindEntities _db;

        public CategoriesModule() : base("api")
        {
            SetupContext();

            Get[@"/Categories"] = r => _db.Categories;
            
            Get[@"/Categories/{id}"] = r => GetCategory(r);
            
            Put[@"/Categories/{id}"] = r => PutCategory(r);
            
            Post[@"/Categories"] = r => PostCategory(r);
               
            Delete[@"/Categories/{id}"] = r =>DeleteCategory(r);
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
                //TODO manca il link
                return HttpStatusCode.Created;
            }
            else
            {
                return HttpStatusCode.BadRequest;
            }
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
            else
            {
                return HttpStatusCode.BadRequest;
            }
        }

        private dynamic GetCategory(dynamic r)
        {
            var category = _db.Categories.Find((int) r.id);
            if (category == null)
            {
                return HttpStatusCode.NotFound;
            }
            return category;
        }

        private void SetupContext()
        {
            _db = new NorthwindEntities();
            _db.Configuration.ProxyCreationEnabled = false;
            JsonSettings.MaxJsonLength = Int32.MaxValue;
        }
    }
}