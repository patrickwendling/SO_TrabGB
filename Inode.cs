using System.Text;
using static trab_GB.Enums;

namespace trab_GB;
public class Inode
{
    public string prop;
    public string grupo;
    public DateTime dtCri;
    public DateTime dtAtu;
    public DateTime dtUltAce;
    public List<string> direitos;
    public bool ehDir;
    public int tamanho;
    public string absNome;
    public List<byte[]> blocos;
    public List<int> indirSimp;

    public Inode(string nome, string prop, bool ehDir = false)
    {
        this.prop = this.grupo = prop;
        this.dtCri = this.dtAtu = this.dtUltAce = DateTime.Now;
        this.ehDir = ehDir;
        this.absNome = nome;
        this.direitos = new List<string> { Convert.ToString(7, 2), Convert.ToString(5, 2), Convert.ToString(0, 2) };
        this.blocos = this.ehDir ? new List<byte[]>() : Enumerable.Repeat(new byte[Utils.LEN_BLOCK], Utils.NUM_BLOCKS_INT).ToList();
        this.indirSimp = new List<int>();
    }

    public override string ToString()
    {
        return $"{RelNome()}";
    }

    public string Info(Dictionary<string, User> listUser, Dictionary<Inode, byte[]> inodes)
    {
        AtualizaTamanho(inodes);
        return $"{ToString()}  {'d' == (ehDir ? 'd' : 'f')}{NormalizaDireitos()}  {tamanho}  {(listUser.ContainsKey(prop) ? listUser[prop].login : prop)} {(listUser.ContainsKey(grupo) ? listUser[grupo].login : grupo)}";
    }

    public string Data()
    {
        return $"{ToString()}  {dtCri:yyyy-MM-dd HH:mm:ss}    {dtAtu:yyyy-MM-dd HH:mm:ss}    {dtUltAce:yyyy-MM-dd HH:mm:ss}";
    }

    public string NormalizaDireitos()
    {
        string padrao = "rwx";
        StringBuilder saida = new StringBuilder();

        foreach (string dir in direitos)
        {
            string strDir = Utils.BinToStrDireito(Convert.ToInt32(dir, 2));

            for (int i = 0; i < strDir.Length; i++)
            {
                saida.Append(strDir[i] == '1' ? padrao[i] : '-');
            }
        }

        return saida.ToString();
    }

    public void SetaDireito(List<int> novosDireitos)
    {
        direitos = novosDireitos.Select(i => Convert.ToString(i, 2)).ToList();
    }

    public void Formata()
    {
        blocos = ehDir ? new List<byte[]>() : Enumerable.Repeat(new byte[Utils.LEN_BLOCK], Utils.NUM_BLOCKS_INT).ToList();
    }

    public void AtualizaTamanho(Dictionary<Inode, byte[]> inodes)
    {
        tamanho = ehDir ? blocos.Count * 4 : blocos.Sum(b => b.Length - Array.FindAll(b, x => x == 0x00).Length);
        if (!ehDir)
        {
            foreach (int n in indirSimp)
            {
                tamanho += inodes[this].Length - Array.FindAll(inodes[this], x => x == 0x00).Length;
            }
        }
    }

    public string Pai()
    {
        if ("/" == absNome)
        {
            return null;
        }

        int ridx = absNome.LastIndexOf("/", 0, absNome.Length - 1);
        return ridx != 0 ? absNome.Substring(0, ridx) : absNome[0].ToString();
    }

    public string RelNome()
    {
        return absNome != "/" ? absNome.Substring(absNome.LastIndexOf("/", 0, absNome.Length - 1) + 1) : "/";
    }

    public bool VerificaPermissao(string userId, Acao acao)
    {
        return (Utils.BinToStrDireito(Convert.ToInt32(direitos[(int)Agente.GERAL]))[Convert.ToInt32(acao)] != '0')
            || (grupo == userId && Utils.BinToStrDireito(Convert.ToInt32(direitos[(int)Agente.GRUPO]))[Convert.ToInt32(acao)] != '0')
            || (prop == userId && Utils.BinToStrDireito(Convert.ToInt32(direitos[(int)Agente.PROPRIETARIO]))[Convert.ToInt32(acao)] != '0');
    }

    public void AtualizaDataAtualizacao()
    {
        dtUltAce = dtAtu = DateTime.Now;
    }

    public void AtualizaDataAcesso()
    {
        dtUltAce = DateTime.Now;
    }
}