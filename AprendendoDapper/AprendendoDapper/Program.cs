using LearningDapper.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using BaltaDataAccess.Models;

namespace LearningDapper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string connectionString = "Server=localhost,1433;Database=balta;User ID=sa;Password=123;TrustServerCertificate=true";

            


            using (var connection = new SqlConnection(connectionString))
            {
                //UpdateCategory(connection);
                //DeleteCategory(connection);
                //CreateCategory(connection);
                //CreateManyCategories(connection);
                //ListCategories(connection);
                //GetCategory(connection);
                //ExecuteProcedure(connection);
                //ExecuteReadProcedure(connection);
                //CreateCategoryWithExecuteScalar(connection);
                //ReadView(connection);
                //OneToOne(connection);
                //OneToMany(connection);
                //QueryMutiple(connection);
                //SelectIn(connection);
                //Like(connection,"backend");
                Transaction(connection);
                
            }
        }

        static void GetCategory(SqlConnection connection)
        {
            var category = connection
                .QueryFirstOrDefault<Category>(
                    "SELECT TOP 1 [Id], [Title] FROM [Category] WHERE [Id]=@id",
                    new
                    {
                        id = "af3407aa-11ae-4621-a2ef-2028b85507c4"
                    });
            Console.WriteLine($"{category.Id} - {category.Title}");

        }

        static void ListCategories(SqlConnection connection) {

            var categories = connection.Query<Category>("SELECT [Id],[Title] from [Category]");
            foreach (var item in categories)
            {
                Console.WriteLine($"{item.Id} - {item.Title}");
            }
        }

        static void CreateCategory(SqlConnection connection)
        {
            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Description = "Categoria destinada a serviços do AWS";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;

            var insertSql = "INSERT INTO [Category] VALUES(@Id,@Title,@Url,@Summary,@Order,@Description,@Featured)";

            var rows = connection.Execute(insertSql, new
            {
                Id = category.Id,
                Title = category.Title,
                Url = category.Url,
                Summary = category.Summary,
                Order = category.Order,
                Description = category.Description,
                Featured = category.Featured
            });

            Console.WriteLine($"{rows} linhas inseridas");
        }

        static void CreateManyCategories(SqlConnection connection)
        {
            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Description = "Categoria destinada a serviços do AWS";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;

            var category2 = new Category();
            category2.Id = Guid.NewGuid();
            category2.Title = "Categoria Nova";
            category2.Url = "categoria-nova";
            category2.Description = "Categoria nova";
            category2.Order = 9;
            category2.Summary = "Categoria";
            category2.Featured = true;

            var insertSql = "INSERT INTO [Category] VALUES(@Id,@Title,@Url,@Summary,@Order,@Description,@Featured)";

            var rows = connection.Execute(insertSql, new[] 
            {
                new
                {
                    Id = category.Id,
                    Title = category.Title,
                    Url = category.Url,
                    Summary = category.Summary,
                    Order = category.Order,
                    Description = category.Description,
                    Featured = category.Featured
                },

                 new
                {
                    Id = category2.Id,
                    Title = category2.Title,
                    Url = category2.Url,
                    Summary = category2.Summary,
                    Order = category2.Order,
                    Description = category2.Description,
                    Featured = category2.Featured
                }
            }

            );

            Console.WriteLine($"{rows} linhas inseridas");
        }

        static void UpdateCategory(SqlConnection connection)
        {
            var updateQuey = "UPDATE [Category] SET [Title] = @Title where [Id] = @Id";
            var rows = connection.Execute(updateQuey, new
            {
                Id = new Guid("AF3407AA-11AE-4621-A2EF-2028B85507C4"),
                Title = "Frontend 2024"
            });

            Console.WriteLine($"{rows} registros atualizados");
        }

        static void DeleteCategory(SqlConnection connection)
        {
            var deleteQuery = "DELETE [Category] WHERE [Id]=@id";
            var rows = connection.Execute(deleteQuery, new
            {
                id = new Guid("92516ED9-E52F-4FD5-9142-849F68803398"),
            });

            Console.WriteLine($"{rows} registros excluídos");
        }

        static void ExecuteProcedure(SqlConnection connection)
        {
            var procedure = "[spDeleteStudent]";

            var pars = new
            {
                StudentId = "4ee8cfeb-d1cc-4b42-bc36-7a5ff8e488e7"
            };

            var affectedRows = connection.Execute(procedure, pars,commandType: CommandType.StoredProcedure);
            
            Console.WriteLine($"{affectedRows} linhas afetadas");
        }

        static void ExecuteReadProcedure(SqlConnection connection)
        {
            var procedure = "[spGetCoursesByCategory]";

            var pars = new
            {
                CategoryId = "09CE0B7B-CFCA-497B-92C0-3290AD9D5142"
            };

            var  courses = connection.Query<Course>(procedure, pars, commandType: CommandType.StoredProcedure);

            foreach ( var item in courses)
            {
                Console.WriteLine(item.Title);
            }


            
        }

        static void CreateCategoryWithExecuteScalar(SqlConnection connection) {
            var category = new Category();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Description = "Categoria destinada a serviços do AWS";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;

            var insertSql = "INSERT INTO [Category] OUTPUT inserted.[Id] VALUES(NEWID(),@Title,@Url,@Summary,@Order,@Description,@Featured)";

            var id = connection.ExecuteScalar<Guid>(insertSql, new
            {
                Title = category.Title,
                Url = category.Url,
                Summary = category.Summary,
                Order = category.Order,
                Description = category.Description,
                Featured = category.Featured
            });

            Console.WriteLine($"A Categoria inserida foi: {id}");
        }

        static void ReadView(SqlConnection connection)
        {
            var sql = "SELECT * FROM [vwCourses]";
            var courses = connection.Query<Course>(sql);
            foreach (var item in courses)
            {
                Console.WriteLine($"{item.Id} - {item.Title}");
            }
        }

        static void OneToOne(SqlConnection connection)
        {
            var sql = "SELECT * FROM [CareerItem] INNER JOIN [Course] ON [CareerItem].[CourseId] = [Course].[Id]";

            var items = connection.Query<CareerItem, Course, CareerItem>(sql, (careerItem, course) =>
            {
                careerItem.Course = course;
                return careerItem;
            }, splitOn: "Id");

            foreach(var item in items)
            {
                Console.WriteLine($"{item.Title} - Curso: {item.Course.Title}");
            }
        }

        static void OneToMany(SqlConnection connection)
        {
            var sql = @"
                SELECT 
                    [Career].[Id],
                    [Career].[Title],
                    [CareerItem].[CareerId],
                    [CareerItem].[Title]
                FROM 
                    [Career] 
                INNER JOIN 
                    [CareerItem] ON [CareerItem].[CareerId] = [Career].[Id]
                ORDER BY
                    [Career].[Title]";

            var careers = new List<Career>();
            var items = connection.Query<Career, CareerItem, Career>(sql, (career, item) =>
            {
                var car = careers.Where(x => x.Id == career.Id).FirstOrDefault();
                if(car == null)
                {
                    car = career;
                    car.Items.Add(item);
                    careers.Add(car);
                }
                else
                {
                    car.Items.Add(item);
                }

                return career;
            }, splitOn: "CareerId");

            foreach (var career in careers)
            {
                Console.WriteLine($"{career.Title}");
                foreach (var item in career.Items)
                {
                    Console.WriteLine($" - {item.Title}");
                }
            }


        }

        static void QueryMutiple(SqlConnection connection) {
            var query = "SELECT * FROM [Category];SELECT * FROM [Course];";
            using(var multi = connection.QueryMultiple(query))
            {
                var categories = multi.Read<Category>();
                var courses = multi.Read<Course>();

                foreach(var item in categories)
                {
                    Console.WriteLine(item.Title);
                }

                foreach (var item in courses)
                {
                    Console.WriteLine(item.Title);
                }

            }
        }

        static void SelectIn(SqlConnection connection)
        {
            var query = @"select * from Career where [Id] IN @Id";

            var items = connection.Query<Career>(query, new
            {
                Id = new[]{
                    "4327ac7e-963b-4893-9f31-9a3b28a4e72b",
                    "e6730d1c-6870-4df3-ae68-438624e04c72"
                }
            });

            foreach (var item in items)
            {
                Console.WriteLine(item.Title);
            }

        }

        static void Like(SqlConnection connection, string term)
        {
            var query = @"SELECT * FROM [Course] WHERE [Title] LIKE @exp";

            var items = connection.Query<Course>(query, new
            {
                exp = $"%{term}%"
            });

            foreach (var item in items)
            {
                Console.WriteLine(item.Title);
            }
        }

        static void Transaction(SqlConnection connection)
        {
            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Minha categoria que não";
            category.Url = "amazon";
            category.Description = "Categoria destinada a serviços do AWS";
            category.Order = 8;
            category.Summary = "AWS Cloud";
            category.Featured = false;

            var insertSql = @"INSERT INTO 
                    [Category] 
                VALUES(
                    @Id, 
                    @Title, 
                    @Url, 
                    @Summary, 
                    @Order, 
                    @Description, 
                    @Featured)";

            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                var rows = connection.Execute(insertSql, new
                {
                    category.Id,
                    category.Title,
                    category.Url,
                    category.Summary,
                    category.Order,
                    category.Description,
                    category.Featured
                }, transaction);

                transaction.Commit();
                //transaction.Rollback();

                Console.WriteLine($"{rows} linhas inseridas");
            }
        }


    }
}
