using System;
using System.Text;
using static trab_GB.Enums;

namespace trab_GB;
class Program
{
    // Padrões de mensagens de retorno
    private const string MSG_COMM_INV = "Comando inválido";
    private const string MSG_QTD_INV_PARAM = "Quantidade de parâmetros inválida!";
    private const string MSG_DIR_EXIST = "Diretório já existente!";
    private const string MSG_ARQ_EXIST = "Arquivo já existente!";
    private const string MSG_DIR_NAO_EXIST = "Diretório inexistente!";
    private const string MSG_ARQ_NAO_EXIST = "Arquivo inexistente!";
    private const string MSG_DIR_ARQ_NAO_EXIST = "Diretório/Arquivo inexistente!";
    private const string MSG_PERM_INS = "Permissão insuficiente!";
    private const string MSG_ARG_INV = "Argumento inválido!";
    private const string MSG_POS_OUT_DISCO = "Posição maior que o tamanho do disco!";
    private const string MSG_USER_DONT_FOUND = "Usuário não encontrado!";

    static void Main(string[] args)
    {
        var userRoot = new User("0", "root");
        var listUser = new Dictionary<string, User> {
            { userRoot.id, userRoot }
        };
        var user = userRoot;

        var root = new Inode("/", userRoot.login, true);
        root.rights = new List<string> { Utils.BinToStr(7), Utils.BinToStr(7), Utils.BinToStr(7) };

        var inode = root;

        var inodes = new Dictionary<string, Inode> {
            { "/", root }
        };


        //Functions
        void Pwd() {
            Console.WriteLine(inode.absName);
        }

        void Mkdir(string[] cmds)
        {
            if (!inode.VerificaPermissao(user.id, Acao.ESCRITA)) {
                Console.WriteLine(MSG_PERM_INS);
                return;
            }

            if (cmds.Length != 2) {
                Console.WriteLine(MSG_QTD_INV_PARAM);
                return;
            }

            string nome = Utils.MontaPossivelNome(inode, cmds[1]);

            if (inodes.ContainsKey(nome) && inodes[nome].isDir) {
                Console.WriteLine(MSG_DIR_EXIST);
                return;
            }
            else {
                inodes[nome] = new Inode(nome, user.id, true);
                inode.blocks.Add(nome);
                inode.AtualizaDataAtualizacao();
            }
        }

        void Rmdir(string[] cmds)
        {
            if (cmds.Length == 2) {
                string nome = Utils.MontaPossivelNome(inode, cmds[1]);

                if (inode.blocks.Contains(nome) && inodes.ContainsKey(nome) && inodes[nome].isDir) {
                    if (inodes[nome].VerificaPermissao(user.id, Acao.ESCRITA)) {
                        inode.blocks.Remove(nome);
                        inodes.Remove(nome);
                        
                        List<string> dirApagar = new List<string>();
                        foreach (string i in inodes.Keys) {
                            if (i.Contains(nome))
                                dirApagar.Add(i);
                        }
                        
                        foreach (string i in dirApagar)
                            inodes.Remove(i);

                        inode.AtualizaDataAtualizacao();
                    }
                    else {
                        Console.WriteLine(MSG_PERM_INS);
                        return;
                    }
                }
                else {
                    Console.WriteLine(MSG_DIR_NAO_EXIST);
                    return;
                }
            }
            else {
                Console.WriteLine(MSG_QTD_INV_PARAM);
                return;
            }
        }

        void Ls(string[] cmds)
        {
            if (cmds.Length != 1 && cmds.Length != 2) {
                Console.WriteLine(MSG_QTD_INV_PARAM);
                return;
            }

            if (cmds.Length == 1) {
                if (!inode.VerificaPermissao(user.id, Acao.LEITURA)) {
                    Console.WriteLine(MSG_PERM_INS);
                    return;
                }
                
                foreach (string sd in inode.blocks) {
                    Console.WriteLine(inodes[sd].Info(listUser, inodes));
                }

                inode.AtualizaDataAcesso();
            }
            else if (cmds.Length == 2) { // Printa o 'ls' de dentro de um diretório
                string nome = Utils.MontaPossivelNome(inode, cmds[1]);

                if (inode.blocks.Contains(nome) && inodes[nome].isDir) {
                    if (inodes[nome].VerificaPermissao(user.id, Acao.LEITURA)) {
                        foreach (string ssd in inodes[nome].blocks) {
                            Console.WriteLine(inodes[ssd].Info(listUser, inodes));
                        }

                        inodes[nome].AtualizaDataAcesso();
                    }
                    else {
                        Console.WriteLine(MSG_PERM_INS);
                        return;
                    }
                }
                else {
                    Console.WriteLine(MSG_DIR_NAO_EXIST);
                    return;
                }
            }
        }

        void Cd(string[] cmds)
        {
            if (cmds.Length == 2) {
                if (cmds[1] == ".." && inode.Pai() != null) {
                    string parent = inode.Pai();

                    if (!inodes.ContainsKey(parent)) {
                        Console.WriteLine(MSG_DIR_NAO_EXIST);
                        return;
                    }
                    if (!inodes[parent].VerificaPermissao(user.id, Acao.EXECUCAO)) {
                        Console.WriteLine(MSG_PERM_INS);
                        return;
                    }
                    inode = inodes[parent];
                    inode.AtualizaDataAcesso();
                    return;
                }

                string nome = Utils.MontaPossivelNome(inode, cmds[1]);
                if (inode.blocks.Contains(nome) && inodes.ContainsKey(nome) && inodes[nome].isDir) {
                    if (!inodes[nome].VerificaPermissao(user.id, Acao.EXECUCAO)) {
                        Console.WriteLine(MSG_PERM_INS);
                        return;
                    }
                    inode = inodes[nome];
                    inode.AtualizaDataAcesso();
                }
                else {
                    Console.WriteLine(MSG_DIR_NAO_EXIST);
                    return;
                }
            }
            else {
                Console.WriteLine(MSG_QTD_INV_PARAM);
                return;
            }
        }

        void Touch(string[] cmds)
        {
            if (!inode.VerificaPermissao(user.id, Acao.ESCRITA)) {
                Console.WriteLine(MSG_PERM_INS);
                return;
            }

            if (cmds.Length != 2) {
                Console.WriteLine(MSG_QTD_INV_PARAM);
                return;
            }

            string nome = Utils.MontaPossivelNome(inode, cmds[1]);

            if (inodes.ContainsKey(nome) && !inodes[nome].isDir) {
                Console.WriteLine(MSG_ARQ_EXIST);
                return;
            }
            else {
                inodes[nome] = new Inode(nome, user.id, false);
                inode.blocks.Add(nome);
                inode.AtualizaDataAtualizacao();
            }
        }

        void Rm(string[] cmds)
        {
            if (cmds.Length == 2) {
                string nome = Utils.MontaPossivelNome(inode, cmds[1]);

                if (inode.blocks.Contains(nome) && inodes.ContainsKey(nome) && !inodes[nome].isDir) {
                    if (inodes[nome].VerificaPermissao(user.id, Acao.ESCRITA)) {
                        inode.blocks.Remove(nome);
                        inodes.Remove(nome);
                        inode.AtualizaDataAtualizacao();
                    }
                    else {
                        Console.WriteLine(MSG_PERM_INS);
                        return;
                    }
                }
                else {
                    Console.WriteLine(MSG_ARQ_NAO_EXIST);
                    return;
                }
            }
            else {
                Console.WriteLine(MSG_QTD_INV_PARAM);
                return;
            }
        }

        void Chmod(string[] cmds)
        {
            if (user.id != userRoot.id) {
                Console.WriteLine(MSG_PERM_INS);
                return;
            }

            if (cmds.Length == 3) {
                string nome = Utils.MontaPossivelNome(inode, cmds[1]);

                if (inode.blocks.Contains(nome)) {
                    if (inodes.ContainsKey(nome)) {
                        inodes[nome].SetaDireito(cmds[2]);
                        inodes[nome].AtualizaDataAtualizacao();
                    }
                    else {
                        Console.WriteLine(MSG_DIR_ARQ_NAO_EXIST);
                        return;
                    }
                }
                else {
                    Console.WriteLine(MSG_DIR_ARQ_NAO_EXIST);
                    return;
                }
            }
            else {
                Console.WriteLine(MSG_QTD_INV_PARAM);
                return;
            }
        }

        void Chown(string[] cmds)
        {
            if (user.id != userRoot.id) {
                Console.WriteLine(MSG_PERM_INS);
                return;
            }

            if (cmds.Length >= 3 && cmds.Length <= 4) {
                string nome = Utils.MontaPossivelNome(inode, cmds[cmds.Length - 1]);

                if (inode.blocks.Contains(nome)) {
                    foreach (User u in listUser.Values) {
                        if (u.login == cmds[1])  {
                            inodes[nome].prop = u.id;
                            inodes[nome].AtualizaDataAtualizacao();
                        }

                        if (cmds.Length == 4 && u.login == cmds[2]) {
                            inodes[nome].grupo = u.id;
                            inodes[nome].AtualizaDataAtualizacao();
                        }
                    }
                }
                else {
                    Console.WriteLine(MSG_DIR_ARQ_NAO_EXIST);
                    return;
                }
            }
            else {
                Console.WriteLine(MSG_QTD_INV_PARAM);
                return;
            }
        }

        void AddUser(string[] cmds)
        {
            if (user.id != userRoot.id) {
                Console.WriteLine(MSG_PERM_INS);
                return;
            }

            if (cmds.Length == 3) {
                if (!int.TryParse(cmds[2], out int userId)) {
                    Console.WriteLine(MSG_ARG_INV);
                    return;
                }

                var strId = userId.ToString();

                if (listUser.ContainsKey(strId)) {
                    Console.WriteLine("Usuário existente!");
                    return;
                }
                else {
                    listUser[strId] = new User(strId, cmds[1]);
                }
            }
            else {
                Console.WriteLine(MSG_QTD_INV_PARAM);
                return;
            }
        }

        void RmUser(string[] cmds)
        {
            if (user.id != userRoot.id) {
                Console.WriteLine(MSG_PERM_INS);
                return;
            }

            if (cmds.Length == 2) {
                if (!int.TryParse(cmds[1], out int userId))
                {
                    Console.WriteLine(MSG_ARG_INV);
                    return;
                }

                var strId = userId.ToString();

                if (userId == 0) {
                    Console.WriteLine("Não é possível remover o root!");
                    return;
                }

                if (listUser.ContainsKey(strId)) {
                    listUser.Remove(strId);
                }
                else {
                    Console.WriteLine("Usuário inexistente!");
                    return;
                }
            }
            else {
                Console.WriteLine(MSG_QTD_INV_PARAM);
                return;
            }
        }

        void LsUser()
        {
            if (user.id != userRoot.id) {
                Console.WriteLine(MSG_PERM_INS);
                return;
            }

            foreach (User u in listUser.Values)
                Console.WriteLine(u);
        }

        void Grava(string[] cmds)
        {
            if (cmds.Length != 5) {
                Console.WriteLine(MSG_QTD_INV_PARAM);
                return;
            }

            string nome = cmds[1];
            string posicao = cmds[2];
            string nbytes = cmds[3];
            string buffer = cmds[4];

            if (!int.TryParse(posicao, out int parsedPosicao) || !int.TryParse(nbytes, out int parsedNbytes)) {
                Console.WriteLine(MSG_ARG_INV);
                return;
            }

            List<string> blocos = null;
            List<string> blocosInd = null;

            string nomeAbs = Utils.MontaPossivelNome(inode, nome);

            if (inodes.ContainsKey(nomeAbs) && !inodes[nomeAbs].isDir) {
                if (inodes[nomeAbs].VerificaPermissao(user.id, Acao.ESCRITA)) {
                    blocos = inodes[nomeAbs].blocks;
                    blocosInd = new List<string>();
                    foreach (var dirBloc in inodes[nomeAbs].indirSimp) {
                        foreach (var block in dirBloc) {
                            blocosInd.Add(block);
                        }
                    }
                }
                else {
                    Console.WriteLine(MSG_PERM_INS);
                    return;
                }
            }

            if (blocos == null) {
                Console.WriteLine(MSG_ARQ_NAO_EXIST);
                return;
            }

            if (parsedPosicao > Utils.LEN_TOTAL)
            {
                Console.WriteLine(MSG_POS_OUT_DISCO);
                return;
            }

            if (buffer.Length != parsedNbytes){
                Console.WriteLine(MSG_ARG_INV);
            }

            Byte[] bufferNumerico = Encoding.ASCII.GetBytes(buffer);
            int idxBlocoInit = parsedPosicao >= Utils.LEN_BLOCK ? parsedPosicao / Utils.LEN_BLOCK : 0;
            int idxByteInit = parsedPosicao >= Utils.LEN_BLOCK ? parsedPosicao % Utils.LEN_BLOCK : parsedPosicao;
            int idxBloco = 0;
            int idxByte = 0;
            int countBytes = 0;

            // if (parsedPosicao + tamParcial > Utils.LEN_TOTAL)
            // {
            //     Console.WriteLine("Block size is larger than the total size of the disk.");
            //     return;
            // }

            inodes[nomeAbs].AtualizaDataAtualizacao();

            while (true)
            {
                if (idxBlocoInit < Utils.NUM_BLOCKS_INT)
                {
                    while (true)
                    {
                        var strToSave = inodes[nomeAbs].blocks[idxBlocoInit];
                        var strToSaveBin = Utils.StringToByteArray(strToSave); 

                        while (true)
                        {
                            strToSaveBin[idxByteInit++] = bufferNumerico[countBytes++];

                            if (countBytes == parsedNbytes) {
                                inodes[nomeAbs].blocks[idxBlocoInit] = BitConverter.ToString(strToSaveBin);
                                break;
                            }

                            if (idxByteInit == Utils.LEN_BLOCK) {
                                idxByteInit = 0;
                                inodes[nomeAbs].blocks[idxBlocoInit] = BitConverter.ToString(strToSaveBin);
                                break;
                            }  
                        }

                        idxBlocoInit++;

                        if (countBytes == parsedNbytes || idxBlocoInit == Utils.NUM_BLOCKS_INT)
                                break;
                    }
                } else{
                    if (inodes[nomeAbs].indirSimp == null)
                        inodes[nomeAbs].indirSimp = new();

                        break;
                }

                if (countBytes == parsedNbytes)
                    break;
            }
        }


        void Cat(string[] cmds)
        {
            if (cmds.Length == 2) {
                string nome = Utils.MontaPossivelNome(inode, cmds[1]);

                if (inodes.ContainsKey(nome) && !inodes[nome].isDir) {
                    if (inodes[nome].VerificaPermissao(user.id, Acao.LEITURA)) {
                        StringBuilder dataAscii = new();
                        foreach(string s in inodes[nome].blocks)
                        {
                            dataAscii.Append(Encoding.ASCII.GetString(Utils.StringToByteArray(s)));
                        }

                        if (inodes[nome].indirSimp.Count() > 0) {
                            var blocksInd = inodes[nome].indirSimp;
                            foreach(List<string> lis in blocksInd) {
                                if (lis != null && lis.Count() > 0) {
                                    foreach (string s in lis) {
                                        dataAscii.Append(Encoding.ASCII.GetString(Utils.StringToByteArray(s)));
                                    }
                                }
                            }
                        }

                        Console.WriteLine(dataAscii);
                        inodes[nome].AtualizaDataAcesso();
                    }
                    else {
                        Console.WriteLine(MSG_PERM_INS);
                        return;
                    }
                }
                else {
                    Console.WriteLine(MSG_ARQ_NAO_EXIST);
                    return;
                }
            }
            else {
                Console.WriteLine(MSG_QTD_INV_PARAM);
                return;
            }
        }

        void Formata(string[] cmds)
        {
            if (cmds.Length == 2) {
                string nome = Utils.MontaPossivelNome(inode, cmds[1]);

                if (inodes.ContainsKey(nome)) {
                    if (inodes[nome].VerificaPermissao(user.id, Acao.ESCRITA)) {
                        inodes[nome].Formata();
                        inodes[nome].AtualizaDataAtualizacao();
                    }
                    else {
                        Console.WriteLine(MSG_PERM_INS);
                        return;
                    }
                }
                else {
                    Console.WriteLine(MSG_ARQ_NAO_EXIST);
                    return;
                }
            }
            else {
                Console.WriteLine(MSG_QTD_INV_PARAM);
                return;
            }
        }

        void Login(string[] cmds)
        {
            if (cmds.Length == 2) {
                if (!int.TryParse(cmds[1], out int userId)) {
                    Console.WriteLine(MSG_ARG_INV);
                    return;
                }

                var strId = userId.ToString();

                if (listUser.ContainsKey(strId)) {
                    user = listUser[strId];
                }
                else {
                    Console.WriteLine(MSG_USER_DONT_FOUND);
                    return;
                }
            }
            else {
                Console.WriteLine(MSG_QTD_INV_PARAM);
                return;
            }
        }

        void Date(string[] cmds)
        {
            if (cmds.Length < 1 || cmds.Length > 2) {
                Console.WriteLine(MSG_QTD_INV_PARAM);
                return;
            }

            if (cmds.Length == 1) {
                if (!inode.VerificaPermissao(user.id, Acao.LEITURA)) {
                    Console.WriteLine(MSG_PERM_INS);
                    return;
                }

                foreach (var sd in inode.blocks) {
                    Console.WriteLine(inodes[sd].Data());
                    inode.AtualizaDataAcesso();
                } 
            }
            else if (cmds.Length == 2) {
                string nome = Utils.MontaPossivelNome(inode, cmds[1]);

                if (inode.blocks.Contains(nome) && inodes[nome].isDir) {
                    if (inodes[nome].VerificaPermissao(user.id, Acao.LEITURA)) {
                        foreach (var ssd in inodes[nome].blocks) {
                            Console.WriteLine(inodes[ssd].Data());
                            inodes[ssd].AtualizaDataAcesso();
                        }
                    }
                    else {
                        Console.WriteLine(MSG_PERM_INS);
                        return;
                    }
                }
                else {
                    Console.WriteLine(MSG_DIR_NAO_EXIST);
                    return;
                }
            }
        }


        // Funcionamento
        while(true) {
            Console.Write(user.login + ":" + inode.absName + "# ");
            string comando = Console.ReadLine();
            string[] cmds = comando.Split(' ');

            if (cmds[0] == "pwd")
                Pwd();
            else if (cmds[0] == "mkdir")
                Mkdir(cmds);
            else if (cmds[0] == "rmdir")
                Rmdir(cmds);
            else if (cmds[0] == "ls")
                Ls(cmds);
            else if (cmds[0] == "cd")
                Cd(cmds);
            else if (cmds[0] == "touch")
                Touch(cmds);
            else if (cmds[0] == "rm")
                Rm(cmds);
            else if (cmds[0] == "chmod")
                Chmod(cmds) ;
            else if (cmds[0] == "chown")
                Chown(cmds);
            else if (cmds[0] == "adduser")
                AddUser(cmds);
            else if (cmds[0] == "lsuser")
                LsUser();
            else if (cmds[0] == "rmuser")
                RmUser(cmds);
            else if (cmds[0] == "grava")
                Grava(cmds);
            else if (cmds[0] == "cat")
                Cat(cmds);
            else if (cmds[0] == "formata")
                Formata(cmds);
            else if (cmds[0] == "login")
                Login(cmds);
            else if (cmds[0] == "date")
                Date(cmds);
            else if (cmds[0] == "clear")
                Console.Clear();
            else
                Console.WriteLine(MSG_COMM_INV);
        }
    }
}
