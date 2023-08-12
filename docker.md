# fastgithub 镜像使用

# 镜像获取

pull 在 Dockerhub 上的镜像

```bash
docker pull marcantoine153/fastgithub:latest
```

## 启动容器

下载项目根目录的 [docker-compose.yml](https://github.com/marc-antoine233/FastGithub/blob/master/docker-compose.yaml)，之后启动容器：

```bash
wget https://github.com/marc-antoine233/FastGithub/blob/master/docker-compose.yaml
docker compose -d
```

启动后会创建 `cacert` 文件夹用于存放 fastgithub 的 crt 证书

## 为系统添加 crt 证书

以 `centOS` 系统为例：

```bash
sudo cp ./cacert/fastgithub.crt /etc/pki/ca-trust/source/anchors/
sudo update-ca-trust
```

## 设置代理

### docker 宿主机设置代理

临时设置 仅对当前终端生效

```bash
export http_proxy=http://localhost:38457 && export https_proxy=http://localhost:38457
```

永久设置 对所有终端生效，在`~/.bashrc`中加入如下字段

```bash
export http_proxy=http://localhost:38457
export https_proxy=http://localhost:38457
```

`source ~/.bashrc` 后生效

### docker 容器设置代理

基本同宿主机设置，但是要先获取宿主机的ip，ip可以是局域网ip

临时设置 仅对当前终端生效

```bash
export http_proxy=http://hostip:38457 && export https_proxy=http://hostip:38457
```

永久设置 对所有终端生效，在`~/.bashrc`中加入如下字段

```bash
export http_proxy=http://hostip:38457
export https_proxy=http://hostip:38457
```

上面的`hostip`要自行修改

## 注意事项

1. 容器启动后，前几次可能会报502 Bad Gateway，这是因为无法及时解析到 github 的 ip，后面用体感一直就没有问题了
2. 其他容器如果不添加 fastgithub 的证书，则在使用 fastgithub 代理的时候容易出现各种 warning，需要自行忽视
