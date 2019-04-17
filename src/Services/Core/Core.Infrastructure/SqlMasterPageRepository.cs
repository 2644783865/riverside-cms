using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Core.Domain;
using static Dapper.SqlMapper;

namespace Riverside.Cms.Services.Core.Infrastructure
{
    public class SqlMasterPageRepository : IMasterPageRepository
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlMasterPageRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        private IEnumerable<MasterPageZone> GetMasterPageZones(IEnumerable<MasterPageZone> masterPageZones, IEnumerable<MasterPageZoneElementDto> masterPageZoneElements, IEnumerable<MasterPageZoneElementTypeDto> masterPageZoneElementTypeIds)
        {
            foreach (MasterPageZone masterPageZone in masterPageZones)
            {
                masterPageZone.MasterPageZoneElements = masterPageZoneElements.Where(e => e.MasterPageZoneId == masterPageZone.MasterPageZoneId).Select(e => new MasterPageZoneElement { ElementId = e.ElementId, ElementTypeId = e.ElementTypeId, MasterPageZoneElementId = e.MasterPageZoneElementId, BeginRender = e.BeginRender, EndRender = e.EndRender });
                masterPageZone.ElementTypeIds = masterPageZoneElementTypeIds.Where(e => e.MasterPageZoneId == masterPageZone.MasterPageZoneId).Select(e => e.ElementTypeId);
                yield return masterPageZone;
            }
        }

        public async Task<MasterPage> ReadMasterPageAsync(long tenantId, long masterPageId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                using (GridReader gr = await connection.QueryMultipleAsync(@"
                    SELECT
                        TenantId,
                        MasterPageId,
                        Name,
                        PageName,
                        PageDescription,
                        AncestorPageId,
                        AncestorPageLevel,
                        PageType,
                        HasOccurred,
                        HasImage,
                        ThumbnailImageWidth,
                        ThumbnailImageHeight,
                        ThumbnailImageResizeMode,
                        PreviewImageWidth,
                        PreviewImageHeight,
                        PreviewImageResizeMode,
                        ImageMinWidth,
                        ImageMinHeight,
	                    Creatable,
                        Deletable,
                        Taggable,
                        Administration,
                        BeginRender,
                        EndRender
                    FROM
                        cms.MasterPage
                    WHERE
                        TenantId = @TenantId AND
                        MasterPageId = @MasterPageId

                    SELECT
                        MasterPageZoneId,
                        AdminType,
                        ContentType,
                        BeginRender,
                        EndRender,
                        Name
                    FROM
                        cms.MasterPageZone
                    WHERE
                        TenantId = @TenantId AND
                        MasterPageId = @MasterPageId
                    ORDER BY
                        SortOrder

                    SELECT
                        cms.MasterPageZoneElement.MasterPageZoneId,
                        cms.MasterPageZoneElement.MasterPageZoneElementId,
                        cms.Element.ElementTypeId,
                        cms.MasterPageZoneElement.ElementId,
                        cms.MasterPageZoneElement.BeginRender,
                        cms.MasterPageZoneElement.EndRender
                    FROM
                        cms.MasterPageZoneElement
                    INNER JOIN
                        cms.Element
                    ON
                        cms.MasterPageZoneElement.TenantId = cms.Element.TenantId AND
                        cms.MasterPageZoneElement.ElementId = cms.Element.ElementId
                    WHERE
                        cms.MasterPageZoneElement.TenantId = @TenantId AND
                        cms.MasterPageZoneElement.MasterPageId = @MasterPageId
                    ORDER BY
                        cms.MasterPageZoneElement.MasterPageZoneId,
                        cms.MasterPageZoneElement.SortOrder,
                        cms.MasterPageZoneElement.MasterPageZoneElementId

                    SELECT
                        MasterPageZoneId,
                        ElementTypeId
                    FROM
                        cms.MasterPageZoneElementType
                    WHERE
                        TenantId = @TenantId AND
                        MasterPageId = @MasterPageId
                    ",
                    new { TenantId = tenantId, MasterPageId = masterPageId }))
                {
                    MasterPage masterPage = await gr.ReadFirstOrDefaultAsync<MasterPage>();
                    if (masterPage != null)
                    {
                        IEnumerable<MasterPageZone> masterPageZones = await gr.ReadAsync<MasterPageZone>();
                        IEnumerable<MasterPageZoneElementDto> masterPageZoneElements = await gr.ReadAsync<MasterPageZoneElementDto>();
                        IEnumerable<MasterPageZoneElementTypeDto> masterPageZoneElementTypeIds = await gr.ReadAsync<MasterPageZoneElementTypeDto>();
                        masterPage.MasterPageZones = GetMasterPageZones(masterPageZones, masterPageZoneElements, masterPageZoneElementTypeIds);
                    }
                    return masterPage;
                }
            }
        }
    }
}
