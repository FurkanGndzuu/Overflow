import NextAuth from "next-auth"
import Keycloak from "next-auth/providers/keycloak"

export const { handlers, auth, signIn, signOut } = NextAuth({
  providers: [
    Keycloak({
      clientId: process.env.AUTH_KEYCLOAK_ID,
      clientSecret: process.env.AUTH_KEYCLOAK_SECRET,
      issuer: process.env.AUTH_KEYCLOAK_ISSUER,
    }),
  ],
  secret: process.env.AUTH_SECRET ?? process.env.NEXTAUTH_SECRET,

  callbacks: {
    async jwt({ token, account }) {
    const now = Math.floor(Date.now() / 1000);
            
            if (account && account.access_token && account.refresh_token) {
                const res = await fetch(`${process.env.AUTH_KEYCLOAK_ISSUER}/realms/overflow/protocol/openid-connect/userinfo`, {
                    headers: {
                        Authorization: `Bearer ${account.access_token}`,
                    }
                });

                if (res.ok) {
                    token.user = await res.json();
                } else {
                    console.error('Failed to fetch user profile: ', await res.text())
                }
                
                token.accessToken = account.access_token
                token.refreshToken = account.refresh_token;
                token.accessTokenExpires = now + account.expires_in!;
                token.error = undefined;
                return token;
            }

            // if access token is still valid then return it
            if (token.accessTokenExpires && now < token.accessTokenExpires) {
                return token;
            }

            try {
                const response = await fetch(`${process.env.AUTH_KEYCLOAK_ISSUER}/protocol/openid-connect/token`, {
                    method: 'POST',
                    headers: {'Content-Type': 'application/x-www-form-urlencoded'},
                    body: new URLSearchParams({
                        grant_type: 'refresh_token',
                        client_id: process.env.AUTH_KEYCLOAK_ID!,
                        client_secret: process.env.AUTH_KEYCLOAK_SECRET!,
                        refresh_token: token.refreshToken as string,
                    })
                })

                const refreshed = await response.json();

                if (!response.ok) {
                    console.log('Failed to refresh token', refreshed);
                    token.error = 'RefreshAccessTokenError';
                    return token;
                }

                token.accessToken = refreshed.access_token
                token.refreshToken = refreshed.refresh_token;
                token.accessTokenExpires = now + refreshed.expires_in!;
            } catch (e) {
                console.log('Failed to refresh token', e);
                token.error = 'RefreshAccessTokenError';
            }

            return token;
    },
    async session({ session, token }) {
     
            
            if (token.accessToken) {
                session.accessToken = token.accessToken;
            }

            if (token.accessTokenExpires) {
                session.expires = new Date(token.accessTokenExpires * 1000) as unknown as typeof session.expires;
            }

            return session;
    }
  }
})