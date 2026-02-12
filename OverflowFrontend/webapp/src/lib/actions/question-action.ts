'use server';
import {Question} from "@/lib/types";
import { fetchClient } from "../fetchClient";

export async function getQuestions(tag?: string) {

    let url = '';


    if(tag !== null && tag !== undefined)
            url = `?tag=${tag}`;

     return await fetchClient<Question[]>(`/questions${url}`, 'GET')


}

export  async function getQuestionById(id: string) {

    return await fetchClient<Question>(`/questions/${id}`, 'GET');

}

export async function SearchQuestions( query: string ) {
    return await fetchClient<Question[]>(`/search?query=${query}`, 'GET');
}