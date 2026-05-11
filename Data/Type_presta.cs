using MySql.Data.MySqlClient;
using RepositoryPOO;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Bibliotheque_classe_projet_Seux
{
    public class Type_presta : BDDObject
    {
        private string nom_type;

        public string Nom_type { get => nom_type; set => nom_type = value; }

        public Type_presta() { }

        public Type_presta(string nom_type) : base()
        {
            this.Nom_type = nom_type;
        }

        public override string GetTableName() => "Type_presta";

        public override Dictionary<string, string> GetInsertUpdateColumns()
        {
            return new Dictionary<string, string>
            {
                { "nom_type", "@nom_type" }
            };
        }

        public override List<IDataParameter> GetInsertUpdateParameters()
        {
            return new List<IDataParameter>
            {
                new MySqlParameter("@nom_type", MySqlDbType.VarChar) { Value = this.Nom_type }
            };
        }

        public override Dictionary<string, string> GetPrimaryKeyColumn()
        {
            return new Dictionary<string, string> { { "id_type", "@id_type" } };
        }

        public override List<IDataParameter> GetPrimaryKeyParameters()
        {
            return new List<IDataParameter>
            {
                new MySqlParameter("@id_type", MySqlDbType.Int32) { Value = Id }
            };
        }

        public override void FillWithDataReader(DbDataReader reader)
        {
            base.FillWithDataReader(reader);
            this.Nom_type = reader["nom_type"].ToString();
        }

        public override Dictionary<string, string> GetPredicateColumns()
        {
            var columns = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(Nom_type))
                columns.Add("nom_type", "@nom_type");
            return columns;
        }

        public override List<IDataParameter> GetPredicateParameters()
        {
            var parameters = new List<IDataParameter>();
            if (!string.IsNullOrEmpty(Nom_type))
                parameters.Add(new MySqlParameter("@nom_type", MySqlDbType.VarChar) { Value = this.Nom_type });
            return parameters;
        }

        public override IBDDConnector getNewInstance() => new Type_presta();
    }
}