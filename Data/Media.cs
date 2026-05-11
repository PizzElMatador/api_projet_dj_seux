using MySql.Data.MySqlClient;
using RepositoryPOO;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Bibliotheque_classe_projet_Seux
{
    public class Media : BDDObject
    {
        private string url_media;
        private int id_type;

        public string Url_media { get => url_media; set => url_media = value; }
        public int Id_type { get => id_type; set => id_type = value; }

        public Media() { }

        public Media(string url_media, int id_type) : base()
        {
            this.Url_media = url_media;
            this.Id_type = id_type;
        }

        public override string GetTableName() => "Media";

        public override Dictionary<string, string> GetInsertUpdateColumns()
        {
            return new Dictionary<string, string>
            {
                { "url_media", "@url_media" },
                { "id_type", "@id_type" }
            };
        }

        public override List<IDataParameter> GetInsertUpdateParameters()
        {
            return new List<IDataParameter>
            {
                new MySqlParameter("@url_media", MySqlDbType.VarChar) { Value = this.Url_media },
                new MySqlParameter("@id_type", MySqlDbType.Int32) { Value = this.Id_type }
            };
        }

        public override Dictionary<string, string> GetPrimaryKeyColumn()
        {
            return new Dictionary<string, string> { { "id_media", "@id_media" } };
        }

        public override List<IDataParameter> GetPrimaryKeyParameters()
        {
            return new List<IDataParameter>
            {
                new MySqlParameter("@id_media", MySqlDbType.Int32) { Value = Id }
            };
        }

        public override void FillWithDataReader(DbDataReader reader)
        {
            base.FillWithDataReader(reader);
            this.Url_media = reader["url_media"].ToString();
            this.Id_type = System.Convert.ToInt32(reader["id_type"]);
        }

        public override Dictionary<string, string> GetPredicateColumns()
        {
            var columns = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(Url_media)) columns.Add("url_media", "@url_media");
            if (Id_type > 0) columns.Add("id_type", "@id_type");
            return columns;
        }

        public override List<IDataParameter> GetPredicateParameters()
        {
            var parameters = new List<IDataParameter>();
            if (!string.IsNullOrEmpty(Url_media))
                parameters.Add(new MySqlParameter("@url_media", MySqlDbType.VarChar) { Value = this.Url_media });
            if (Id_type > 0)
                parameters.Add(new MySqlParameter("@id_type", MySqlDbType.Int32) { Value = this.Id_type });
            return parameters;
        }

        public override IBDDConnector getNewInstance() => new Media();
    }
}