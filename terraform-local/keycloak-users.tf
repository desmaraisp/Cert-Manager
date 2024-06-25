resource "keycloak_user" "admin_user" {
  realm_id = keycloak_realm.realm.id
  username = "root"
  enabled  = true

  email = "root@domain.com"
  initial_password {
    value     = "123"
    temporary = false
  }
}

resource "keycloak_user" "foo_user" {
  realm_id = keycloak_realm.realm.id
  username = "foo"
  enabled  = true

  email = "foo@domain.com"
  initial_password {
    value     = "123"
    temporary = false
  }
}
resource "keycloak_user_roles" "foo_roles" {
  realm_id = keycloak_realm.realm.id
  user_id  = keycloak_user.foo_user.id

  role_ids = [
    keycloak_role.foo_role.id,
  ]
}

resource "keycloak_user" "bar_user" {
  realm_id = keycloak_realm.realm.id
  username = "bar"
  enabled  = true

  email = "bar@domain.com"
  initial_password {
    value     = "123"
    temporary = false
  }
}
resource "keycloak_user_roles" "bar_roles" {
  realm_id = keycloak_realm.realm.id
  user_id  = keycloak_user.bar_user.id

  role_ids = [
    keycloak_role.bar_role.id,
  ]
}