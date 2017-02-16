namespace URLHasher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUpvote : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Upvotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VoterId = c.String(maxLength: 128),
                        VotedURLId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.URLs", t => t.VotedURLId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.VoterId)
                .Index(t => t.VoterId)
                .Index(t => t.VotedURLId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Upvotes", "VoterId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Upvotes", "VotedURLId", "dbo.URLs");
            DropIndex("dbo.Upvotes", new[] { "VotedURLId" });
            DropIndex("dbo.Upvotes", new[] { "VoterId" });
            DropTable("dbo.Upvotes");
        }
    }
}
