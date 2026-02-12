export type Question = {
    id: string
    title: string
    context: string
    askerId: string
    askerName?: string
    createdAt: string
    updatedAt?: string
    views: number
    tagSlugs: string[]
    hasAcceptedAnswer: boolean
    votes: number
    answerCount: number
    answers: Answer[]
    userVoted: number;
}

export type Answer = {
    id: string
    content: string
    userId: string
    userDisplayName: string
    createdAt: string
    updatedAt?: string
    accepted: boolean
    questionId: string
    votes: number
    userVoted: number;
}
export type Tag = {
    id: string
    name: string
    slug: string
    description : string
}