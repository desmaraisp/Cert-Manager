resource "keycloak_realm" "realms" {
  for_each = var.KEYLCOAK_REALMS
  realm    = each.key
  enabled  = true
  attributes = {
    organization_id = each.key
  }
}


resource "keycloak_user" "user_with_initial_password" {
  for_each = keycloak_realm.realms
  realm_id = each.value.id
  username = "alice"
  enabled  = true

  email = "alice@domain.com"

  initial_password {
    value     = "VERY_BAD_PASSWORD"
    temporary = true
  }
}


resource "keycloak_openid_client" "client_credentials_clients" {
  for_each  = keycloak_realm.realms
  realm_id  = each.value.id
  client_id = "${each.value.id}-client"

  name        = "${each.value.id} client"
  enabled     = true
  web_origins = ["*"]

  access_type = "CONFIDENTIAL"
  valid_redirect_uris = [
    "http://localhost:3222/openid-callback"
  ]

  client_secret                = "BAD_CLIENT_SECRET"
  standard_flow_enabled        = true
  service_accounts_enabled     = true
  direct_access_grants_enabled = true
}

resource "keycloak_openid_audience_protocol_mapper" "audience_mapper" {
  for_each = keycloak_openid_client.client_credentials_clients
  realm_id = each.value.realm_id

  client_id = each.value.client_id
  name      = "audience-mapper"

  included_custom_audience = "cert-manager"
}

resource "keycloak_openid_client_scope" "cert-read-scope" {
  for_each               = keycloak_realm.realms
  realm_id               = each.value.id
  name                   = "cert-manager/read"
  description            = "Allows read-only access to certManager api"
  include_in_token_scope = true
}
resource "keycloak_openid_client_scope" "cert-write-scope" {
  for_each               = keycloak_realm.realms
  realm_id               = each.value.id
  name                   = "cert-manager/write"
  description            = "Allows read-only access to certManager api"
  include_in_token_scope = true
}

resource "keycloak_openid_client_optional_scopes" "client_optional_scopes" {
  count     = length(var.KEYLCOAK_REALMS)
  realm_id  = keycloak_realm.realms[count.index].id
  client_id = keycloak_openid_client.client_credentials_clients[count.index].id

  optional_scopes = [
    keycloak_openid_client_scope.cert-write-scope[count.index].name,
    keycloak_openid_client_scope.cert-read-scope[count.index].name
  ]
}
