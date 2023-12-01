using System.Text;
using static trab_GB.Enums;

namespace trab_GB;
public class Inode
{
    public string prop;
    public string grupo;
    public DateTime dtCre;
    public DateTime dtUpd;
    public DateTime dtUltAcc;
    public List<string> rights;
    public bool isDir;
    public int size;
    public string absName;
    public List<string> blocks;
    public List<string> indirSimp;

    public Inode(string name, string prop, bool isDir = false)
    {
        this.prop = this.grupo = prop;
        this.dtCre = this.dtUpd = this.dtUltAcc = DateTime.Now;
        this.isDir = isDir;
        this.absName = name;
        this.rights = new List<string> { Utils.BinToStr(7), Utils.BinToStr(5), Utils.BinToStr(0) };
        this.blocks = isDir ? new List<string>() : Enumerable.Repeat(new byte[Utils.LEN_BLOCK], Utils.NUM_BLOCKS_INT).Select(b => BitConverter.ToString(b)).ToList();
        this.indirSimp = new List<string>();
    }

    public override string ToString()
    {
        return $"{RelName()}";
    }

    public void AtualizaDataAtualizacao()
    {
        this.dtUltAcc = this.dtUpd = DateTime.Now;
    }

    public void AtualizaDataAcesso()
    {
        this.dtUltAcc = DateTime.Now;
    }

    public void AtualizaSize(Dictionary<string, Inode> inodes)
    {
        // Console.WriteLine(this.blocks.Count());
        // Console.WriteLine(this.blocks[0]);
        this.size = this.isDir ? this.blocks.Count() * 4 : this.blocks.Sum(b => Utils.StringToByteArray(b).Length - Utils.StringToByteArray(b).Count(c => c == '\x00')); 
        if (!this.isDir)
        {
            foreach (string n in this.indirSimp)
            {
                this.size += inodes[n].blocks.Sum(b => Utils.StringToByteArray(b).Length - Utils.StringToByteArray(b).Count(c => c == '\x00'));
            }
        }
    }

    public string Info(Dictionary<string, User> listUser, Dictionary<string, Inode> inodes)
    {
        AtualizaSize(inodes);
        return $"{ToString()}  {(this.isDir ? 'd' : '-')}{NormalRights()}  {this.size}  {(listUser.ContainsKey(this.prop) ? listUser[this.prop].login : this.prop)} {(listUser.ContainsKey(this.grupo) ? listUser[this.grupo].login : this.grupo)}";
    }

    public string Data()
    {
        return $"{ToString()}  {this.dtCre:yyyy-MM-dd HH:mm:ss}    {this.dtUpd:yyyy-MM-dd HH:mm:ss}    {this.dtUltAcc:yyyy-MM-dd HH:mm:ss}";
    }

    public string NormalRights()
    {
        string padrao = "rwx";
        StringBuilder saida = new();

        foreach (string strDir in this.rights) {
            for (int i = 0; i < strDir.Length; i++) {
                saida.Append(strDir[i] == '1' ? padrao[i] : '-');
            }
        }

        return saida.ToString();
    }

    public void SetaDireito(string rights)
    {
        List<string> newRights = new();
        for (int i = 0; i < rights.Length; i++) {
            newRights.Add(Utils.BinToStr(Convert.ToInt32(rights[i].ToString())));
        }
        this.rights = newRights;
    }

    public void Formata()
    {
        this.blocks = this.isDir ? new List<string>() : Enumerable.Repeat(new byte[Utils.LEN_BLOCK], Utils.NUM_BLOCKS_INT).Select(b => BitConverter.ToString(b)).ToList();
    }

    public string Pai()
    {
        if ("/" == this.absName)
        {
            return null;
        }

        int ridx = this.absName.LastIndexOf("/", 0);
        return ridx != 0 ? this.absName.Substring(0, ridx) : this.absName[0].ToString();
    }

    public string RelName()
    {
        if ("/" != this.absName) {
            int index = this.absName.LastIndexOf("/", 0) + 1;
            return this.absName.Substring(index);
        }
        else {
            return "/";
        }
    }

    public bool VerificaPermissao(string userId, Acao acao) {
        return ((this.rights[(int)Agente.GERAL])[Convert.ToInt32(acao)] != '0')
            || (this.grupo == userId && (this.rights[(int)Agente.GRUPO])[Convert.ToInt32(acao)] != '0')
            || (this.prop == userId && (this.rights[(int)Agente.PROPRIETARIO])[Convert.ToInt32(acao)] != '0');
    }
}