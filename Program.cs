using System;

namespace trab_GB;
class Program
{
    // Padrões de mensagens de retorno
    private const string MSG_QTD_INV_PARAM = "Quantidade de parâmetros inválida!";
    private const string MSG_DIR_EXIST = "Diretório já existente!";
    private const string MSG_ARQ_EXIST = "Arquivo já existente!";
    private const string MSG_DIR_NAO_EXIST = "Diretório inexistente!";
    private const string MSG_ARQ_NAO_EXIST = "Arquivo inexistente!";
    private const string MSG_DIR_ARQ_NAO_EXIST = "Diretório/Arquivo inexistente!";
    private const string MSG_PERM_INS = "Permissão insuficiente!";
    private const string MSG_ARG_INV = "Argumento inválido!";

    static void Main(string[] args)
    {
        var userRoot = new User("root");
        var listUsers = new List<User>() { userRoot };
        var user = userRoot;
        
        Console.WriteLine(MSG_QTD_INV_PARAM);

        // while(true) {
            // var comando = Console.ReadLine(user.nome + ":" + inode.absNome + "# ");
            // var cmds = comando.Split();

            // if (cmds[0] == "pwd")
            //     pwd()
            // else if (cmds[0] == "mkdir")
            //     mkdir(cmds)
            // else if (cmds[0] == "touch")
            //     touch(cmds)
            // else if (cmds[0] == "ls")
            //     ls(cmds)
            // else if (cmds[0] == "cd")
            //     cd(cmds)
            // else if (cmds[0] == "rm")
            //     rm(cmds)
            // else if (cmds[0] == "rmdir")
            //     rmdir(cmds)
            // else if (cmds[0] == "adduser")
            //     adduser(cmds)
            // else if (cmds[0] == "rmuser")
            //     rmuser(cmds)
            // else if (cmds[0] == "lsuser")
            //     lsuser()
            // else if (cmds[0] == "c")
            //     os.system('clear')
            // else if (cmds[0] == "chown")
            //     chown(cmds)
            // else if (cmds[0] == "chmod")
            //     chmod(cmds)   
            // else if cmds[0] == "grava")
            //     grava(cmds)
            // else if (cmds[0] == "cat")
            //     cat(cmds)
            // else if (cmds[0] == "formata")
            //     formata(cmds)
            // else if (cmds[0] == "login")
            //     login(cmds)
            // else if (cmds[0] == "lsd")
            //     lsd(cmds)
            // else if (cmds[0] == "print")
            //     print(inodes)
        // }
    }
}
