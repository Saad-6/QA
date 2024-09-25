using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.F.A.Q.Domain;
using Nop.Data.Extensions;
using Nop.Core.Domain.Localization;

namespace Nop.Plugin.F.A.Q.Migrations;
[NopSchemaMigration("2024/08/01 08:40:55:1687541", "Testing.HelloWorld base schema", MigrationProcessType.Installation)]
public class SchemaMigration : Migration
{
    public override void Up()
    {
        Create.TableFor<FAQEntity>();
    }
    public override void Down()
    {
        Delete.Table("FAQEntity");
        
    }

}
