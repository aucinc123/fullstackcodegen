using Dapper;
using DapperCommonScenarios.API.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DapperCommonScenarios.API.Services.Data
{
    public class BlogService
    {
        private readonly AppSettings appSettings;

        public BlogService(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        public async Task<List<Blog>> GetBlogListAsync()
        {
            using (SqlConnection cn = new SqlConnection(appSettings.DBConnection))
            {
                var query = await cn.QueryAsync<Blog>($"dbo.uspGetBlogList"
                                                    , new
                                                    {
                                                    }
                                                    , commandType: CommandType.StoredProcedure);
                return query.AsList();
            }
        }

        public async Task<Blog> GetBlogAsync(int blogId)
        {
            using (SqlConnection cn = new SqlConnection(appSettings.DBConnection))
            {
                var query = await cn.QueryAsync<Blog>($"dbo.uspGetBlog"
                                                    , new
                                                    {
                                                        @Id = blogId
                                                    }
                                                    , commandType: CommandType.StoredProcedure);
                return query.FirstOrDefault();
            }
        }

        public async Task<Blog> InsertBlogAsync(Blog blog)
        {
            using (SqlConnection cn = new SqlConnection(appSettings.DBConnection))
            {
                blog.Id = await cn.ExecuteScalarAsync<int>($"dbo.uspInsertBlog",
                                        new
                                        {
                                            @Title = blog.Title,
                                            @Contents = blog.Contents
                                        },
                                        commandType: CommandType.StoredProcedure);

                return blog;
            }
        }

        public async Task<bool> UpdateBlogAsync(int blogId, Blog blog)
        {
            using (SqlConnection cn = new SqlConnection(appSettings.DBConnection))
            {
                var doesExist = await cn.ExecuteScalarAsync<bool>($"dbo.uspUpdateBlog",
                                        new
                                        {
                                            @Id = blog.Id,
                                            @Title = blog.Title,
                                            @Contents = blog.Contents
                                        }, commandType: CommandType.StoredProcedure);
                return doesExist;
            }
        }

        public async Task<Blog> GetBlogWithComments(int blogId)
        {
            using (SqlConnection cn = new SqlConnection(appSettings.DBConnection))
            {
                var reader = await cn.QueryMultipleAsync($"dbo.uspGetBlogWithComments"
                                                        , new
                                                        {
                                                            @Id = blogId
                                                        }
                                                        , commandType: CommandType.StoredProcedure);

                var blogQuery = await reader.ReadAsync<Blog>();
                var blog = blogQuery.FirstOrDefault();

                if (blog != null)
                {
                    var commentsQuery = await reader.ReadAsync<BlogComment>();
                    blog.BlogCommentList = commentsQuery.ToList();
                }

                return blog;
            }
        }

        public async Task<List<BlogComment>> SaveBlogCommentsListAsync(int blogId, List<BlogComment> blogCommentList)
        {
            var blogCommentTable = GetBlogCommentTable(blogCommentList);

            using (SqlConnection cn = new SqlConnection(appSettings.DBConnection))
            {
                var commentQuery = await cn.QueryAsync<BlogComment>($"dbo.uspSaveBlogCommentList",
                                                        new
                                                        {
                                                            @BlogId = blogId,
                                                            @BlogCommentTable = blogCommentTable
                                                        },
                                                        commandType: CommandType.StoredProcedure);

                return commentQuery.ToList();
            }
        }

        private DataTable GetBlogCommentTable(List<BlogComment> blogCommentList)
        {
            var table = new DataTable("BlogCommentsTable");

            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Comment", typeof(string));
            table.Columns.Add("NumberOfStars", typeof(int));

            foreach (var blogComment in blogCommentList)
            {
                table.Rows.Add(blogComment.Id,
                                blogComment.Comment,
                                blogComment.NumberOfStars);        
            }

            return table;
        }
    }
}
