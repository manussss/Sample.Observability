- [Configurando a aplicação com Prometheus e Grafana](#configurando-a-aplicação-com-prometheus-e-grafana)
  - [Configurar Grafana](#configurar-grafana)
    - [Altere o arquivo docker-compose](#altere-o-arquivo-docker-compose)
  - [Configurar Prometheus](#configurar-prometheus)
    - [Altere o arquivo prometheus.yml](#altere-o-arquivo-prometheusyml)
  - [Execute o docker-compose](#execute-o-docker-compose)
  - [Acesso ao Prometheus](#acesso-ao-prometheus)
    - [Targets](#targets)
  - [Configurar Grafana \& Prometheus](#configurar-grafana--prometheus)
- [Problemas conhecidos](#problemas-conhecidos)
  - [Container do Prometheus não consegue enxergar sua aplicação](#container-do-prometheus-não-consegue-enxergar-sua-aplicação)


# Configurando a aplicação com Prometheus e Grafana

Essa configuração se baseia nas seguintes premissas:
- Você está utilizando uma máquina windows
- Possui o WSL instalado e configurado
- Possui o Docker instalado e configurado
- Possui uma conta no Grafana (incluso conta free-cloud)

## Configurar Grafana

1. Acesse sua conta Grafana
2. Acesse `Connections`
3. `Private data source connect`
4. `Add new network`
5. Defina um nome
6. Selecione a configuração de sua preferência, neste caso, `Docker`
7. Gere um novo token
8. O Grafana vai gerar um comando para você subir uma imagem no docker, salve este comando para realizar as substituições abaixo

### Altere o arquivo docker-compose

Altere o arquivo `docker-compose.yml` realizando as substituições abaixo:

1. Substitua `<SEU-TOKEN>` pelo token gerado na interface do Grafana
2. Substituia `<SEU-CLUSTER>` pelo cluster utilizado no Grafana
3. Substitua `<SEU-ID>` pelo seu ID no Grafana

## Configurar Prometheus

### Altere o arquivo prometheus.yml

Substitua `111.111.1.11` pelo seu IP e `5000` pela porta da sua aplicação.

Foi necessário colocar o meu `IPV4`, obtido através do comando `ipconfig`.

## Execute o docker-compose

Vá até a pasta `docker` do repositório e execute o comando abaixo para subir os containers do agente do Grafana e do Prometheus:

`docker-compose up -d`

## Acesso ao Prometheus

Você pode acessar a GUI do Prometheus no endereço e porta configurados, neste caso:

`http://localhost:9090/`

### Targets

Quando sua aplicação subir, você pode validar que o Prometheus consegue obter suas métricas acessando:

`http://localhost:9090/targets`

![image](https://github.com/user-attachments/assets/8b6c4e01-b4df-432a-ac44-7eda738117cf)

## Configurar Grafana & Prometheus

1. Teste a conexão do agente no Grafana

![image](https://github.com/user-attachments/assets/bfdf0098-6c16-4f64-8191-36f4f3441510)

2. Clique em `create a new data source`
3. Selecione `Prometheus`
4. Em `Connection`, defina: `http://<nome-container>:9090`, substituindo `<nome-container>` com o nome do seu container do Prometheus
5. Em `HTTP Method` selecione `GET`
6. Em `Private data source connect` selecione sua conexão

![image](https://github.com/user-attachments/assets/67f8bb50-d2ac-4d7c-8e7f-07e4c6aa5ff0)

7. Clique em `Save & Test`

Com estes passos, é possível visualizar as métricas da aplicação em `Explore`:

![image](https://github.com/user-attachments/assets/f8003b58-0f9c-442e-a122-af4cd0a99a58)


# Problemas conhecidos

## Container do Prometheus não consegue enxergar sua aplicação

Ao executar a aplicação em .net na máquina windows e o prometheus estar em um container no docker, foi necessário adicionar as linhas de código abaixo:

``` csharp
builder.WebHost.UseUrls("http://0.0.0.0:5260");
```

Sendo "5260" a porta da minha aplicação.
