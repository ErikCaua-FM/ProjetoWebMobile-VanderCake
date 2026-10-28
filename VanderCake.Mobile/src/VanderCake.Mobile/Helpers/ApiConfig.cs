namespace VanderCake.Helpers;

public static class ApiConfig
{
    // Para aparelho físico, preencha, por exemplo: "http://192.168.1.10:5080/api/".
    // Deixe vazio para usar automaticamente o emulador Android ou o Windows local.
    private const string EnderecoComputadorNaRede = "";
    // No celular físico, troque pelo IP do computador na mesma rede.
    // A API deverá escutar a rede e liberar a porta 5080 no firewall.
    public static string BaseUrl
    {
        get
        {
#if DEBUG
            if (!string.IsNullOrWhiteSpace(EnderecoComputadorNaRede)) return EnderecoComputadorNaRede;
            return DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5080/api/"
                : "http://localhost:5080/api/";
#else
            // Substitua pelo domínio HTTPS real antes de distribuir.
            return "https://api.vandercake.example/api/";
#endif
        }
    }
}
