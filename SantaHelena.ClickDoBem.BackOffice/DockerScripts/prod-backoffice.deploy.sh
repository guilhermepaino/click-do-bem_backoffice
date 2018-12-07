#!/bin/bash

GIT_REP="https://gitlab.s2it.com.br/santa-helena/click-do-bem_backoffice.git"
API_SERVER=http://clickdobemapi.santahelena.com

echo "Preprando ambiente"
echo "--------------------------------------------------------------------------"
rm -rf /tmp/clickdobem-backoffice
cd /tmp

echo "Obtendo repositorio"
echo "--------------------------------------------------------------------------"

git clone $GIT_REP clickdobem-backoffice
cd clickdobem-backoffice
git checkout master

CID=$(docker ps -a | grep clickdobem | awk '{ print $1 }')
CIDSIZE=${#CID}
if [ $CIDSIZE -gt 0 ];
then
        echo "Removendo container"
        echo "--------------------------------------------------------------------------"
        docker rm clickdobem-backoffice -f
fi

IID=$(docker image list | grep santa-helena/clickdobem-backoffice | awk '{ print $3 }')
IIDSIZE=${#IID}
if [ $IIDSIZE -gt 0 ];
then
        echo "Removendo imagem"
        echo "--------------------------------------------------------------------------"
        docker rmi santa-helena/clickdobem-backoffice --force
fi

echo "Gerando Imagem"
echo "--------------------------------------------------------------------------"
docker build -t santa-helena/clickdobem-backoffice -f SantaHelena.ClickDoBem.BackOffice/Dockerfile .

echo "Iniciando Container"
echo "--------------------------------------------------------------------------"
docker run --name clickdobem-backoffice -e API_SERVER=$API_SERVER -p 5081:80 -d santa-helena/clickdobem-backoffice

echo "Excluindo arquivos temporarios"
cd /
rm -rf /tmp/clickdobem-backoffice

echo "Finalizado"