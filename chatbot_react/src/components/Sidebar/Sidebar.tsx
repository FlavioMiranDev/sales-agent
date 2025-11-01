import { useContext, useEffect } from "react";
import "./Sidebar.scss";
import { useParams } from "react-router-dom";
import { MessageContext } from "../../contexts/message.context";

export function Sidebar() {
  const { id } = useParams<{ id: string }>();
  const { conversationSelected, setConversation, conversations } =
    useContext(MessageContext);

  useEffect(() => {
    setConversation(id || null);
  }, [id]);

  return (
    <aside className="sidebar">
      <div className="logo">
        <div className="logo-icon">AI</div>
        <span className="logo-text">ChatBot</span>
      </div>

      <button className="new-chat-btn" onClick={() => setConversation(null)}>
        <svg
          width="20"
          height="20"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
        >
          <path d="M12 5v14M5 12h14" />
        </svg>
        <span>Novo Chat</span>
      </button>

      <nav className="chat-history">
        {conversations.map((conversation) => (
          <div
            key={conversation.convesationId}
            className={`history-item ${
              conversationSelected === conversation.convesationId
                ? "active"
                : ""
            }`}
            onClick={() => setConversation(conversation.convesationId)}
          >
            <div className="history-text">
              {conversation.title.slice(0, 25)}...
            </div>
          </div>
        ))}
      </nav>

      <div className="user-section">
        <div className="user-info">
          <div className="user-avatar">U</div>
          <div className="user-name">Usuário</div>
        </div>
      </div>
    </aside>
  );
}
