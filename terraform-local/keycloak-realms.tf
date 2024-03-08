resource "keycloak_realm" "realm" {
  realm    = "first_realm"
  enabled  = true
}

resource "keycloak_group" "foo" {
  realm_id = keycloak_realm.realm.id
  name     = "foo"
}
resource "keycloak_group" "bar" {
  realm_id = keycloak_realm.realm.id
  name     = "bar"
}

resource "keycloak_openid_client_scope" "cert-read-scope" {
  realm_id               = keycloak_realm.realm.id
  name                   = "cert-manager/read"
  description            = "Allows read-only access to certManager api"
  include_in_token_scope = true
}
resource "keycloak_openid_client_scope" "cert-write-scope" {
  realm_id               = keycloak_realm.realm.id
  name                   = "cert-manager/write"
  description            = "Allows read-only access to certManager api"
  include_in_token_scope = true
}

