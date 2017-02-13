namespace URLHasher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFKToClick : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clicks", "URLId", c => c.Int(nullable: false));
            CreateIndex("dbo.Clicks", "URLId");
            AddForeignKey("dbo.Clicks", "URLId", "dbo.URLs", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Clicks", "URLId", "dbo.URLs");
            DropIndex("dbo.Clicks", new[] { "URLId" });
            DropColumn("dbo.Clicks", "URLId");
        }
    }
}
