
import { fetchClient } from "../fetchClient";
import { Tag } from "../types";

export async function TagAction(){
    return await fetchClient<Tag[]>('/tags' , 'GET' , {cache: 'force-cache' , next : {revalidate: 60 * 60 * 24}});
}