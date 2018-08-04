using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Core.Infrastructure
{
    public class SqlTagRepository : ITagRepository
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlTagRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<Tag> ReadTagAsync(long tenantId, long tagId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<Tag>(@"
                    SELECT
                        cms.Tag.TagId,
                        cms.Tag.Name
                    FROM
                        cms.Tag
                    WHERE
                        TenantId = @TenantId AND
                        TagId = @TagId",
                    new
                    {
                        TenantId = tenantId,
                        TagId = tagId
                    }
                );
            }
        }

        public async Task<IEnumerable<Tag>> ListTagsAsync(long tenantId, IEnumerable<long> tagIds)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryAsync<Tag>(@"
                    SELECT
                        cms.Tag.TagId,
                        cms.Tag.Name
                    FROM
                        cms.Tag
                    WHERE
                        TenantId = @TenantId AND
                        TagId IN @TagIds",
                    new
                    {
                        TenantId = tenantId,
                        TagIds = tagIds
                    }
                );
            }
        }
    }
}
