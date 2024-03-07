#!/bin/sh
for i in $(env | grep CERTMANAGER_VITE_)
do
    key=$(echo $i | cut -d '=' -f 1)
    value=$(echo $i | cut -d '=' -f 2-)
    echo $key=$value

    # sed JS and CSS only
    find /usr/share/nginx/html -type f \( -name '*.js' -o -name '*.css' \) -exec sed -i "s|${key}|${value}|g" '{}' +
done

if [ -z "${CERTMANAGER_TOML}" ]; then
    echo $CERTMANAGER_TOML | base64 -d > /usr/share/nginx/html/config.toml
fi