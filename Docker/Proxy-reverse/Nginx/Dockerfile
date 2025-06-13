FROM nginx

COPY /Config/nginx.conf /etc/nginx/nginx.conf

COPY /Config/sites-available/* /etc/nginx/sites-available/

COPY /Config/sites-enabled/* /etc/nginx/sites-enabled/

RUN mkdir -p /var/www/monerokoszudikas.duckdns.org/.well-known/acme-challenge/

#RUN apt-get update && apt install -y python3 python3-dev python3-venv libaugeas-dev gcc
#
#RUN python3 -m venv /opt/certbot/ && /opt/certbot/bin/pip install --upgrade pip
#
#RUN /opt/certbot/bin/pip install certbot certbot-nginx
#
#RUN ln -s /opt/certbot/bin/certbot /usr/bin/certbot

#RUN certbot certonly --standalone -d monerokoszudikas.duckdns.org --agree-tos --email badmonero@proton.me
