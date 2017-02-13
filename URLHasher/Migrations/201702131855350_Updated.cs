namespace URLHasher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.URLs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        Long = c.String(),
                        Short = c.String(),
                        Created = c.DateTime(nullable: false),
                        OwnerId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId)
                .Index(t => t.OwnerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.URLs", "OwnerId", "dbo.AspNetUsers");
            DropIndex("dbo.URLs", new[] { "OwnerId" });
            DropTable("dbo.URLs");
        }
    }
}
