'use server';

import { auth } from "@/auth";
import { fetchClient } from "../fetchClient";

export async  function AuthAction() {

    return await fetchClient<string>(`/test/auth`, 'GET' )

}

export async function getCurrentUser(){

    const session = await auth();

    try {
        if (!session?.user) 
            return null;
        return session.user;
    }
    catch (error) {
        console.error("Error fetching user session:", error);
        return null;
    }

}