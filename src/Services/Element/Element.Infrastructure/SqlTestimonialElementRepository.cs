using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Element.Domain;
using static Dapper.SqlMapper;

namespace Riverside.Cms.Services.Element.Infrastructure
{
    public class SqlTestimonialElementRepository : IElementRepository<TestimonialElementSettings>
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlTestimonialElementRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<TestimonialElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                using (GridReader gr = await connection.QueryMultipleAsync(@"
                    SELECT
                        cms.Element.TenantId,
                        cms.Element.ElementId,
                        cms.Element.ElementTypeId,
                        cms.Element.Name,
                        element.Testimonial.DisplayName,
                        element.Testimonial.Preamble
                    FROM
                        cms.Element
                    INNER JOIN
                        element.Testimonial
                    ON
                        cms.Element.TenantId = element.Testimonial.TenantId AND
                        cms.Element.ElementId = element.Testimonial.ElementId
                    WHERE
                        cms.Element.TenantId = @TenantId AND
                        cms.Element.ElementId = @ElementId
                    SELECT
                        element.TestimonialComment.TestimonialCommentId AS TestimonialId,
	                    element.TestimonialComment.Comment,
	                    element.TestimonialComment.Author,
	                    element.TestimonialComment.AuthorTitle,
                        element.TestimonialComment.CommentDate AS Date
                    FROM
                        element.TestimonialComment
                    WHERE
	                    element.TestimonialComment.TenantId = @TenantId AND
                        element.TestimonialComment.ElementId = @ElementId
                    ORDER BY
                        element.TestimonialComment.SortOrder",
                    new
                    {
                        TenantId = tenantId,
                        ElementId = elementId
                    }
                ))
                {
                    TestimonialElementSettings settings = await gr.ReadFirstOrDefaultAsync<TestimonialElementSettings>();
                    if (settings != null)
                        settings.Testimonials = await gr.ReadAsync<Testimonial>();
                    return settings;
                }
            }
        }
    }
}
