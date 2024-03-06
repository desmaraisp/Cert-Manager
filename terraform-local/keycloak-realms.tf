resource "keycloak_realm" "realms" {
  for_each = var.KEYLCOAK_REALMS
  realm    = each.key
  enabled  = true
}


resource "keycloak_user" "user_with_initial_password" {
  for_each = keycloak_realm.realms
  realm_id = each.value.id
  username = "alice"
  enabled  = true

  email = "alice@domain.com"
  initial_password {
    value     = "123"
    temporary = false
  }
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

