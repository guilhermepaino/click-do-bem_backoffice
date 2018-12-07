#!/bin/bash

GIT_REP="https://gitlab.s2it.com.br/santa-helena/click-do-bem_backoffice.git"
API_SERVER=http://dev-clickdobemapi.santahelena.com

echo "Preprando ambiente"
echo "--------------------------------------------------------------------------"
rm -rf /tmp/dev-clickdobem-backoffice
cd /tmp

echo "Obtendo repositorio"
echo "--------------------------------------------------------------------------"

git clone $GIT_REP dev-clickdobem-backoffice
cd dev-clickdobem-backoffice
git checkout develop

CID=$(docker ps -a | grep clickdobem | awk '{ print $1 }')
CIDSIZE=${#CID}
if [ $CIDSIZE -gt 0 ];
then
        echo "Removendo container"
        echo "--------------------------------------------------------------------------"
        docker rm dev-clickdobem-backoffice -f
fi

IID=$(docker image list | grep santa-helena/dev-clickdobem-backoffice | awk '{ print $3 }')
IIDSIZE=${#IID}
if [ $IIDSIZE -gt 0 ];
then
        echo "Removendo imagem"
        echo "--------------------------------------------------------------------------"
        docker rmi santa-helena/dev-clickdobem-backoffice --force
fi

echo "Gerando Imagem"
echo "--------------------------------------------------------------------------"
docker build -t santa-helena/dev-clickdobem-backoffice -f SantaHelena.ClickDoBem.BackOffice/Dockerfile .

echo "Iniciando Container"
echo "--------------------------------------------------------------------------"
docker run --name dev-clickdobem-backoffice -e API_SERVER=$API_SERVER -p 5081:80 -d santa-helena/dev-clickdobem-backoffice

echo "Excluindo arquivos temporarios"
cd /
rm -rf /tmp/dev-clickdobem-backoffice

echo "Finalizado"