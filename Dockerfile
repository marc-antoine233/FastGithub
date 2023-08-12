FROM ubuntu:18.04
LABEL MAINTAINER=marc-antoine153

RUN <<EOF
apt update
apt install libicu-dev libgssapi-krb5-2 libssl-dev -y --fix-missing
apt install -y locales && rm -rf /var/lib/apt/lists/*
localedef -i zh_CN -c -f UTF-8 -A /usr/share/locale/locale.alias zh_CN.UTF-8
EOF

ENV LANG zh_CN.utf8

ADD ./zip/fastgithub_linux-x64 /fastgithub
EXPOSE 38457
CMD /fastgithub/fastgithub