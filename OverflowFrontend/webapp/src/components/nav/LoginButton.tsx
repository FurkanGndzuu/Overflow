"use client";

import { Button } from "@heroui/react";
import { signIn } from "next-auth/react";




export default function SearchInput() {

   

  return(
    <Button
            color='secondary'
            variant='bordered'
            onPress={() => signIn('keycloak', 
                {redirectTo: '/questions'} ,{prompt: 'login'})}
        >
            Login
        </Button>
        
    );
}